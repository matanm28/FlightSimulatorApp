using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlightSimulatorApp.Model {
    using System.ComponentModel;
    using System.Diagnostics;
    using FlightSimulatorApp.Utilities;
    using System.Net.Sockets;
    using System.Threading;

    public class FlightGearTCPHandler : ITCPHandler {

        /// <summary>
        /// Flight Gear's input (get) properties.
        /// </summary>
        public enum FG_InputProperties {
            Heading,
            VerticalSpeed,
            GroundSpeed,
            AirSpeed,
            GpsAltitude,
            InternalRoll,
            InternalPitch,
            AltimeterAltitude,
            Longitude,
            Latitude
        }

        /// <summary>
        /// Flight Gear's output (set) properties.
        /// </summary>
        public enum FG_OutputProperties { Throttle, Rudder, Elevator, Aileron }

        private BiDictionary<FG_InputProperties, string> getParamPath;
        private Dictionary<FG_OutputProperties, string> setParamPath;
        private IList<Thread> threadsList;
        private string buffer = string.Empty;
        private ITelnetClient client;
        private volatile bool stopped;
        private const string Delimiter = "\r\n/>";

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected {
            get { return this.client.IsConnected(); }
            private set {
                if (!value) {
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsConnected)));
                }
            }
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public FlightGearTCPHandler(ITelnetClient client) {
            this.client = client;
            this.threadsList = new List<Thread>();
            this.initializeParametersMap();
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public FlightGearTCPHandler()
            : this(new TelnetClientV2()) {
        }
        /// <summary>
        /// Initializes the parameters map.
        /// </summary>
        private void initializeParametersMap() {
            // get parameters
            this.getParamPath = new BiDictionary<FG_InputProperties, string>();
            this.getParamPath.Add(
                FG_InputProperties.Heading,
                "/instrumentation/heading-indicator/indicated-heading-deg");
            this.getParamPath.Add(FG_InputProperties.VerticalSpeed, "/instrumentation/gps/indicated-vertical-speed");
            this.getParamPath.Add(FG_InputProperties.GroundSpeed, "/instrumentation/gps/indicated-ground-speed-kt");
            this.getParamPath.Add(
                FG_InputProperties.AirSpeed,
                "/instrumentation/airspeed-indicator/indicated-speed-kt");
            this.getParamPath.Add(FG_InputProperties.GpsAltitude, "/instrumentation/gps/indicated-altitude-ft");
            this.getParamPath.Add(
                FG_InputProperties.InternalRoll,
                "/instrumentation/attitude-indicator/internal-roll-deg");
            this.getParamPath.Add(
                FG_InputProperties.InternalPitch,
                "/instrumentation/attitude-indicator/internal-pitch-deg");
            this.getParamPath.Add(
                FG_InputProperties.AltimeterAltitude,
                "/instrumentation/altimeter/indicated-altitude-ft");
            this.getParamPath.Add(FG_InputProperties.Longitude, "/position/longitude-deg");
            this.getParamPath.Add(FG_InputProperties.Latitude, "/position/latitude-deg");

            // set parameters
            this.setParamPath = new Dictionary<FG_OutputProperties, string>();
            this.setParamPath.Add(FG_OutputProperties.Throttle, "/controls/engines/current-engine/throttle ");
            this.setParamPath.Add(FG_OutputProperties.Rudder, "/controls/flight/rudder ");
            this.setParamPath.Add(FG_OutputProperties.Aileron, "/controls/flight/aileron ");
            this.setParamPath.Add(FG_OutputProperties.Elevator, "/controls/flight/elevator ");
        }

        /// <summary>Connects the specified ip.</summary>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        /// <exception cref="System.TimeoutException">if connection took longer then 15s to establish</exception>
        public void Connect(string ip, int port) {
            TimeOutTimer timer = new TimeOutTimer(5);
            string error = string.Empty;
            timer.Start();
            while (!this.client.IsConnected() && !timer.TimePassed) {
                try {
                    this.client.Connect(ip, port);
                } catch (SocketException socketException) {
                    Console.WriteLine(socketException);
                    error = "Remote socket unavailable";
                    continue;
                } catch (ArgumentOutOfRangeException argumentOutOfRangeException) {
                    Console.WriteLine(argumentOutOfRangeException);
                    error = "Port number out of range";
                    continue;
                } catch (Exception e) {
                    Console.WriteLine(e);
                    error = "General Error";
                }
            }

            if (timer.TimePassed && !this.client.IsConnected()) {
                throw new TimeoutException(error);
            }
        }
        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public void Disconnect() {
            this.Stop();
            while (this.threadsLive()) {
                Thread.Sleep(1000);
            }

            this.client.Disconnect();
            this.threadsList = new List<Thread>();
            this.buffer = string.Empty;
        }
        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start() {
            this.stopped = false;
            Thread sendDataRequestsThread = new Thread(this.sendDataRequests);
            sendDataRequestsThread.Name = "sendDataRequestsThread";
            sendDataRequestsThread.Start();
            Thread fillBufferThread = new Thread(this.fillBuffer);
            fillBufferThread.Name = "fillBufferThread";
            fillBufferThread.Start();
            this.threadsList.Add(sendDataRequestsThread);
            this.threadsList.Add(fillBufferThread);
        }

        /// <summary>Stops this instance.</summary>
        public void Stop() {
            this.stopped = true;
        }
        /// <summary>
        /// Sends the specified string.
        /// </summary>
        /// <param name="str">The string.</param>
        private void send(string str) {
            try {
                this.client.Send(str);
            } catch (Exception e) {
                Debug.WriteLine(e);
                this.IsConnected = false;
            }
        }
        /// <summary>
        /// Sets the parameter value.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="value">The value.</param>
        public void SetParameterValue(FG_OutputProperties param, double value) {
            this.send("set " + this.setParamPath[param] + value.ToString() + " \r\n");
        }
        /// <summary>
        /// Checks if the thread is alive.
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
            int count = 0;
            while (!this.stopped) {
                try {
                    this.buffer += this.client.Read();
                    if (this.buffer.Length > 10000) {
                        Thread.Sleep(1000);
                    }
                } catch (Exception e) {
                    if (count > 15) {
                        this.IsConnected = this.client.IsConnected();
                    } else {
                        Console.WriteLine(e);
                        count++;
                        continue;
                    }
                }
            }
        }
        /// <summary>
        /// Sends the data requests.
        /// </summary>
        private void sendDataRequests() {
            while (!this.stopped) {
                foreach (KeyValuePair<FG_InputProperties, string> item in this.getParamPath) {
                    this.send("get " + item.Value + " \r\n");
                }
                Thread.Sleep(250);
            }
        }
        /// <summary>
        /// Reads the data.
        /// </summary>
        /// <returns></returns>
        public IList<string> Read() {
            IList<string> dataVector = null;
            bool gotData = false;
            while (!gotData) {
                if (this.buffer.Contains(Delimiter)) {
                    int index = this.buffer.IndexOf(Delimiter);
                    string line = this.buffer.Substring(0, index + Delimiter.Length + 1);
                    this.buffer = this.buffer.Replace(line, string.Empty);
                    dataVector = this.parseData(line);
                    if (dataVector != null) {
                        gotData = true;
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
        /// <summary>
        /// Trims the data.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="charsToTrim">The chars to trim.</param>
        /// <param name="trimSpaces">if set to <c>true</c> [trim spaces].</param>
        /// <returns></returns>
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

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;
        /// <inheritdoc />
        public string Error {
            //todo: fix
            get { return string.Empty; }
            set { return;}
        }
    }

}