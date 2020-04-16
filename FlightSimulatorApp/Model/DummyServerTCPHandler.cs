using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlightSimulatorApp.Model {
    using FlightSimulatorApp.Utilities;
    using System.Net.Sockets;
    using System.Threading;
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
        private Mutex locker = new Mutex();
        private int counter = 0;

        /// <summary>
        /// Occurs when [disconnect occurred].
        /// </summary>
        public event OnDisconnectEventHandler DisconnectOccurred;

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected => this.client.IsConnected();

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public DummyServerTCPHandler(ITelnetClient client) {
            this.client = client;
            this.threadsList = new List<Thread>();
            this.parsingQueue = new Queue<string>();
            this.initializeParametersMap();
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

            if (timer.TimePassed && !this.client.IsConnected()) {
                throw new TimeoutException(error);
            }
        }

        /// <inheritdoc />
        public void Disconnect() {
            this.Stop();
            while (this.threadsLive())
                Thread.Sleep(1000);

            this.client.Disconnect();
            this.threadsList = new List<Thread>();
            this.buffer = string.Empty;
            this.parsingQueue.Clear();
        }
        /// <inheritdoc />
        public void Start() {
            this.stopped = false;
            Thread sendDataRequestsThread = new Thread(this.sendDataRequests);

            Thread fillBufferThread = new Thread(this.fillBuffer);
            sendDataRequestsThread.Name = "sendDataRequestsThread";
            sendDataRequestsThread.Start();

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
            } catch (Exception exception) {
                this.DisconnectOccurred?.Invoke(exception.Message);
            }
        }
        
        /// <summary>
        /// Sets the parameter value.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="value">The value.</param>
        public void SetParameterValue(setProperties param, double value) {
            try {
                this.send("set " + this.setParamPath[param] + value.ToString() + " \r\n");
                this.buffer += this.setParamPath[param] + " '" + this.client.Read().Replace('\n', '\'') + " \r\n/>";
            } catch (Exception e) {
                this.DisconnectOccurred?.Invoke(e.ToString());
            }
        }

        /// <summary>
        /// Threadses the live.
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
                } catch (Exception e) {
                    Console.WriteLine(e);
                    continue;
                }
            }
        }

        /// <summary>
        /// Sends the data requests.
        /// </summary>
        private void sendDataRequests() {
            while (!this.stopped) {
                foreach (KeyValuePair<getProperties, string> item in this.getParamPath) {
                    try {
                        this.send("get " + item.Value + " \r\n");
                        this.buffer += item.Value + " = '" + this.client.Read().Replace('\n', '\'')
                                       + " (double) \r\n/>";
                    } catch (Exception e) {
                        this.DisconnectOccurred?.Invoke(e.ToString());
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
            while (!gotData) {
                if (this.parsingQueue.Count > 0) {
                    string line = this.parsingQueue.Dequeue();
                    dataVector = this.parseData(line);
                    if (dataVector != null) {
                        gotData = true;
                    } else if (this.counter > 10) {
                        this.counter = 0;
                        this.buffer = string.Empty;
                        Thread.Sleep(50);
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