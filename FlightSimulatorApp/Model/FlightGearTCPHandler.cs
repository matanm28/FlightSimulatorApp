using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.Model {
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows;
    using FlightSimulatorApp.Utilities;
    using Timer = System.Timers.Timer;

    public class FlightGearTCPHandler : ITCPHandler {
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

        public enum FG_OutputProperties { Throttle, Rudder, Elevator, Aileron }

        private BiDictionary<FG_InputProperties, string> getParamPath;
        private Dictionary<FG_OutputProperties, string> setParamPath;
        private IList<Thread> threadsList;
        private string buffer = string.Empty;
        private ITelnetClient client;
        private volatile bool stopped;
        private const string Delimiter = "\r\n/>";

        public event OnDisconnectEventHandler DisconnectOccured;
        public bool IsConnected {
            get { return this.client.isConnected(); }
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
        public void connect(string ip, int port) {
            TimeOutTimer timer = new TimeOutTimer(5);
            string error = string.Empty;
            timer.Start();
            while (!this.client.isConnected() && !timer.TimePassed) {
                try {
                    this.client.connect(ip, port);
                } catch (SocketException socketException) {
                    error = "Remote socket unavailable";
                    continue;
                } catch (ArgumentOutOfRangeException argumentOutOfRangeException) {
                    error = "Port number out of range";
                    continue;
                } catch (Exception e) {
                    error = "General Error";
                }
            }

            if (timer.TimePassed && !this.client.isConnected()) {
                throw new TimeoutException(error);
            }
        }

        public void disconnect() {
            this.stop();
            while (this.threadsLive()) {
                Thread.Sleep(1000);
            }

            this.client.disconnect();
            this.threadsList = new List<Thread>();
        }

        public void start() {
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

        public void stop() {
            this.stopped = true;
        }

        private void send(string str) {
            try {
                this.client.send(str);
            } catch (Exception exception) {
                this.DisconnectOccured?.Invoke(exception.Message);
            }
        }

        public void setParameterValue(FG_OutputProperties param, double value) {
            this.send("set " + this.setParamPath[param] + value.ToString() + " \r\n");
        }

        private bool threadsLive() {
            foreach (Thread thread in this.threadsList) {
                if (thread.IsAlive) {
                    return true;
                }
            }

            return false;
        }

        private void fillBuffer() {
            int count = 0;
            while (!this.stopped) {
                try {
                    this.buffer += this.client.read();
                    if (this.buffer.Length > 10000) {
                        Thread.Sleep(1000);
                    }
                } catch (Exception e) {
                    if (count > 15) {
                        DisconnectOccured?.Invoke(e.Message);
                    } else {
                        Console.WriteLine(e);
                        count++;
                        continue;
                    }
                }
            }
        }

        private void sendDataRequests() {
            while (!this.stopped) {
                foreach (KeyValuePair<FG_InputProperties, string> item in this.getParamPath) {
                    this.send("get " + item.Value + " \r\n");
                    Thread.Sleep(250);
                }
            }
        }

        public IList<string> read() {
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