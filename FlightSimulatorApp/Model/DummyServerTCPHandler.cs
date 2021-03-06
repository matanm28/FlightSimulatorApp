﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlightSimulatorApp.Model {
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;
    using FlightSimulatorApp.Utilities;
    using getProperties = FlightGearTCPHandler.FG_InputProperties;
    using setProperties = FlightGearTCPHandler.FG_OutputProperties;

    /// <summary>
    /// a <see cref="ITCPHandler"/> implementation designed to work with dummyServer.py
    /// </summary>
    /// <seealso cref="FlightSimulatorApp.Model.ITCPHandler" />
    public class DummyServerTCPHandler : ITCPHandler {
        private const string Delimiter = "\r\n/>";
        private BiDictionary<getProperties, string> getParamPath;
        private Dictionary<setProperties, string> setParamPath;
        private IList<Thread> threadsList;
        private string buffer = string.Empty;
        private Queue<string> parsingQueue;
        private ITelnetClient client;
        private volatile bool stopped;
        private int counter;
        private string error = string.Empty;

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        public string Error {
            get { return this.error; }
            set {
                if (value != null && this.error != value) {
                    this.error = value;
                    this.notifyPropertyChanged(nameof(this.Error));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyServerTCPHandler"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        private DummyServerTCPHandler(ITelnetClient client) {
            this.client = new DummyTelnetClient();
            this.threadsList = new List<Thread>();
            this.parsingQueue = new Queue<string>();
            this.initializeParametersMap();
        }

        /// <inheritdoc />
        public bool IsConnected {
            get { return this.client.IsConnected(); }
            private set {
                if (!value) {
                    this.notifyPropertyChanged(nameof(this.IsConnected));
                }
            }
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public DummyServerTCPHandler()
            : this(new DummyTelnetClient()) {
        }

        /// <summary>
        /// Initializes the parameters map.
        /// </summary>
        private void initializeParametersMap() {
            // get parameters
            this.getParamPath = new BiDictionary<getProperties, string>();
            this.getParamPath.Add(
                FlightGearTCPHandler.FG_InputProperties.Heading,
                "/instrumentation/heading-indicator/indicated-heading-deg");
            this.getParamPath.Add(
                FlightGearTCPHandler.FG_InputProperties.VerticalSpeed,
                "/instrumentation/gps/indicated-vertical-speed");
            this.getParamPath.Add(
                FlightGearTCPHandler.FG_InputProperties.GroundSpeed,
                "/instrumentation/gps/indicated-ground-speed-kt");
            this.getParamPath.Add(
                FlightGearTCPHandler.FG_InputProperties.AirSpeed,
                "/instrumentation/airspeed-indicator/indicated-speed-kt");
            this.getParamPath.Add(
                FlightGearTCPHandler.FG_InputProperties.GpsAltitude,
                "/instrumentation/gps/indicated-altitude-ft");
            this.getParamPath.Add(
                FlightGearTCPHandler.FG_InputProperties.InternalRoll,
                "/instrumentation/attitude-indicator/internal-roll-deg");
            this.getParamPath.Add(
                FlightGearTCPHandler.FG_InputProperties.InternalPitch,
                "/instrumentation/attitude-indicator/internal-pitch-deg");
            this.getParamPath.Add(
                FlightGearTCPHandler.FG_InputProperties.AltimeterAltitude,
                "/instrumentation/altimeter/indicated-altitude-ft");
            this.getParamPath.Add(FlightGearTCPHandler.FG_InputProperties.Longitude, "/position/longitude-deg");
            this.getParamPath.Add(FlightGearTCPHandler.FG_InputProperties.Latitude, "/position/latitude-deg");

            // set parameters
            this.setParamPath = new Dictionary<setProperties, string>();
            this.setParamPath.Add(
                FlightGearTCPHandler.FG_OutputProperties.Throttle,
                "/controls/engines/current-engine/throttle ");
            this.setParamPath.Add(FlightGearTCPHandler.FG_OutputProperties.Rudder, "/controls/flight/rudder ");
            this.setParamPath.Add(FlightGearTCPHandler.FG_OutputProperties.Aileron, "/controls/flight/aileron ");
            this.setParamPath.Add(FlightGearTCPHandler.FG_OutputProperties.Elevator, "/controls/flight/elevator ");
        }

        private void notifyPropertyChanged(string propName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        /// <summary>Connects the specified IP.</summary>
        /// <param name="ip">The I.P.</param>
        /// <param name="port">The port.</param>
        /// <exception cref="System.TimeoutException">if connection took longer then 5s to establish</exception>
        public void Connect(string ip, int port) {
            TimeOutTimer timer = new TimeOutTimer(5);
            string error = string.Empty;
            timer.Start();
            while (!this.client.IsConnected() && !timer.TimePassed) {
                try {
                    this.client.Connect(ip, port);
                    this.IsConnected = true;
                    this.Error = string.Empty;
                }
                catch (SocketException socketException) {
                    Console.WriteLine(socketException);
                    error = "Remote socket unavailable";
                    continue;
                }
                catch (ArgumentOutOfRangeException argumentOutOfRangeException) {
                    Console.WriteLine(argumentOutOfRangeException);
                    error = "Port number out of range";
                    continue;
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                    error = "General Error";
                }
            }

            if (timer.TimePassed && !this.client.IsConnected()) {
                this.IsConnected = false;
                this.Error = error;
            }
        }

        /// <inheritdoc />
        public void Disconnect() {
            this.Stop();
            while (this.threadsLive())
                Thread.Sleep(1000);

            this.client.Disconnect();
            this.IsConnected = false;
            this.threadsList = new List<Thread>();
            this.buffer = string.Empty;
            this.parsingQueue = new Queue<string>();
        }

        /// <inheritdoc />
        public void Start() {
            this.stopped = false;
            Thread sendDataRequestsThread = new Thread(this.sendDataRequests);
            Thread fillBufferThread = new Thread(this.fillBuffer);
            sendDataRequestsThread.Name = "sendDataRequestsThread";
            sendDataRequestsThread.Start();
            fillBufferThread.Name = nameof(fillBufferThread);
            fillBufferThread.Start();
            this.threadsList.Add(sendDataRequestsThread);
            this.threadsList.Add(fillBufferThread);
        }

        /// <inheritdoc />
        public void Stop() {
            this.stopped = true;
        }

        /// <summary>Sends the specified string.</summary>
        /// <param name="str">The string.</param>
        private void send(string str) {
            try {
                this.client.Send(str);
            }
            catch (Exception exception) {
                Debug.WriteLine(exception);
                this.Error = "Server got Disconnected";
                this.IsConnected = false;
            }
        }

        /// <summary>
        /// Sets the parameter value.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="value">The value.</param>
        public async void SetParameterValue(setProperties param, double value) {
            try {
                this.send("set " + this.setParamPath[param] + value.ToString() + " \r\n");
                string data = await this.client.Read().ConfigureAwait(false);
                this.buffer += this.setParamPath[param] + " '" + data.Replace('\n', '\'') + " \r\n/>";
                this.Error = string.Empty;
            }
            catch (TimeoutException e) {
                Debug.WriteLine(e);
                this.Error = "Server is a bit slow";
            }
            catch (IOException e) {
                Debug.WriteLine(e);
                if (!this.stopped) {
                    this.IsConnected = false;
                    this.Error = string.Empty;
                }
            }
        }

        /// <summary>
        /// checks to see if any threads are alive.
        /// </summary>
        /// <returns></returns>
        private bool threadsLive() {
            foreach (Thread thread in this.threadsList) {
                if (thread.IsAlive) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Fills the buffer.
        /// </summary>
        private void fillBuffer() {
            while (!this.stopped) {
                try {
                    if (this.buffer.Contains(Delimiter)) {
                        int index = this.buffer.IndexOf(Delimiter);
                        string line = this.buffer.Substring(0, index + Delimiter.Length);
                        this.buffer = this.buffer.Replace(line, string.Empty);
                        this.parsingQueue.Enqueue(line);
                    }

                    if (this.parsingQueue.Count > 100) {
                        Thread.Sleep(1000 * 2);
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                    continue;
                }
            }
        }

        /// <summary>
        /// Sends the data requests.
        /// </summary>
        private async void sendDataRequests() {
            while (!this.stopped) {
                foreach (KeyValuePair<getProperties, string> item in this.getParamPath) {
                    try {
                        this.send("get " + item.Value + " \r\n");
                        string data = await this.client.Read().ConfigureAwait(false);
                        this.buffer += item.Value + " = '" + data.Replace('\n', '\'') + " (double) \r\n/>";
                        this.Error = string.Empty;
                    }
                    catch (TimeoutException e) {
                        Debug.WriteLine(e);
                        this.Error = "Server is a bit slow";
                    }
                    catch (IOException e) {
                        Debug.WriteLine(e);
                        if (!this.stopped) {
                            this.IsConnected = false;
                            this.Error = string.Empty;
                        }

                        return;
                    }
                }

                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Reads the data.
        /// </summary>
        /// <returns></returns>
        public IList<string> Read() {
            IList<string> dataVector = null;
            bool gotData = false;
            while (!gotData && !this.stopped) {
                if (this.parsingQueue.Count > 0) {
                    string line = this.parsingQueue.Dequeue();
                    dataVector = this.parseData(line);
                    if (dataVector != null) {
                        gotData = true;
                    } else if (this.counter > 100) {
                        this.counter = 0;
                        this.buffer = string.Empty;
                        Thread.Sleep(500);
                    } else {
                        this.counter++;
                    }
                }
            }

            return dataVector;
        }

        /// <summary>
        /// Parses the data.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        private IList<string> parseData(string line) {
            IList<string> dataVector = new List<string>(3);
            string[] lineArr = line.Split(" ".ToCharArray());
            foreach (string str in lineArr) { // "path" = 'value' (casting)\r\n/>
                if (this.getParamPath.ContainsValue(str)) {
                    dataVector.Add(this.getParamPath[str].ToString());
                    continue;
                } else if (str.Contains('\'')) {
                    dataVector.Add(str.Trim("\'".ToCharArray()));
                    continue;
                } else if (str.Contains('(')) {
                    dataVector.Add(trimData(str, "()" + Delimiter));
                    continue;
                }
            }

            if (dataVector.Count != 3) {
                dataVector = null;
            }

            return dataVector;
        }

        private static string trimData(string word, string charsToTrim, bool trimSpaces = false) {
            StringBuilder stringBuilder = new StringBuilder();
            if (trimSpaces) {
                charsToTrim = charsToTrim + " ";
            }

            foreach (char c in word) {
                if (!charsToTrim.Contains(c)) {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }
    }
}