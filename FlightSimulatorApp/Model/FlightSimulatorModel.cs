using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maps.MapControl.WPF;

namespace FlightSimulatorApp.Model {
    using System.Collections;
    using System.ComponentModel;
    using System.IO;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using FlightGearInput = FlightGearTCPHandler.FG_InputProperties;
    using FlightGearOutput = FlightGearTCPHandler.FG_OutputProperties;
    using Status = Controls.ConnectionControl.Status;

    public class FlightSimulatorModel : IFlightSimulatorModel {
        private const double TOLERANCE = 0.0001;
        private const double MIN_LATITUDE = -90.0;
        private const double MAX_LATITUDE = 90.0;
        private const double MIN_LONGITUDE = -180.0;
        private const double MAX_LONGITUDE = 180.0;
        private volatile bool running = false;
        private double heading;
        private double verticalSpeed;
        private double groundSpeed;
        private double airSpeed;
        private double gpsAltitude;
        private double internalRoll;
        private double internalPitch;
        private double altimeterAltitude;
        private double throttle;
        private double rudder;
        private double elevator;
        private double aileron;
        private double longitude;
        private double latitude;
        private string errorBoundaries;
        private bool latitudeError;
        private bool longitudeError;
        private string ipAddress;
        private int port;
        private Status connectionStatus = Status.inActive;

        private ITCPHandler tcpHandler;

        public event PropertyChangedEventHandler PropertyChanged;

        public FlightSimulatorModel() {
            this.tcpHandler = new FlightGearTCPHandler(new TelnetClientV2());
            this.tcpHandler.DisconnectOccured += delegate(string error) {
                this.ConnectionStatus = Status.disconnect;
            };
        }

        public FlightSimulatorModel(ITCPHandler tcpHandler) {
            this.tcpHandler = tcpHandler;
            this.tcpHandler.DisconnectOccured += delegate (string error) {
                this.ConnectionStatus = Status.disconnect;
            };
        }

        /// <summary>Connects the specified ip.</summary>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        /// <exception cref="TimeoutException">if model wasn't able to connect</exception>
        public bool Connect(string ip, int port) {
            try {
                this.tcpHandler.connect(ip, port);
                this.resendSetValues();
                return true;
            } catch (TimeoutException timeoutException) {
                return false;
            }
        }

        private void resendSetValues() {
            
                this.tcpHandler.setParameterValue(FlightGearOutput.Throttle, this.Throttle);
                this.tcpHandler.setParameterValue(FlightGearOutput.Rudder, this.Rudder);
                this.tcpHandler.setParameterValue(FlightGearOutput.Aileron, this.Aileron);
                this.tcpHandler.setParameterValue(FlightGearOutput.Elevator, this.Elevator);
            
        }
        
        public async void Disconnect() {
            if (this.connectionStatus != Status.inActive) {
                this.running = false;
                await Task.Run(()=>this.tcpHandler.Disconnect());
                this.ConnectionStatus = Status.inActive;
            }
        }

        public async void Start() {
            bool flag = false;
            if (!this.tcpHandler.IsConnected) {
               flag = await Task<bool>.Run(() => this.Connect(this.IpAddress,this.Port));
            }
            if (flag) {
                this.running = true;
                await Task.Run(() => this.tcpHandler.start());
                Thread runThread = new Thread(this.run);
                runThread.Name = "runThread";
                runThread.Start();
                this.ConnectionStatus = Status.active;
            } else {
                this.ConnectionStatus = Status.inActive;
            }
        }

        private void run() {
            while (this.running) {
                try {
                    IList<string> dataVector = this.tcpHandler.read();
                    FlightGearInput property = stringToEnum(dataVector[0]);
                    switch (property) {
                        case FlightGearInput.Heading:
                            if (dataVector[2].Equals("double", StringComparison.CurrentCultureIgnoreCase)) {
                                this.Heading = double.Parse(dataVector[1]);
                            }

                            break;
                        case FlightGearInput.VerticalSpeed:
                            if (dataVector[2].Equals("double", StringComparison.CurrentCultureIgnoreCase)) {
                                this.VerticalSpeed = double.Parse(dataVector[1]);
                            }

                            break;
                        case FlightGearInput.GroundSpeed:
                            if (dataVector[2].Equals("double", StringComparison.CurrentCultureIgnoreCase)) {
                                this.GroundSpeed = double.Parse(dataVector[1]);
                            }

                            break;
                        case FlightGearInput.AirSpeed:
                            if (dataVector[2].Equals("double", StringComparison.CurrentCultureIgnoreCase)) {
                                this.AirSpeed = double.Parse(dataVector[1]);
                            }

                            break;
                        case FlightGearInput.GpsAltitude:
                            if (dataVector[2].Equals("double", StringComparison.CurrentCultureIgnoreCase)) {
                                this.GpsAltitude = double.Parse(dataVector[1]);
                            }

                            break;
                        case FlightGearInput.InternalRoll:
                            if (dataVector[2].Equals("double", StringComparison.CurrentCultureIgnoreCase)) {
                                this.InternalRoll = double.Parse(dataVector[1]);
                            }

                            break;
                        case FlightGearInput.InternalPitch:
                            if (dataVector[2].Equals("double", StringComparison.CurrentCultureIgnoreCase)) {
                                this.InternalPitch = double.Parse(dataVector[1]);
                            }

                            break;
                        case FlightGearInput.AltimeterAltitude:
                            if (dataVector[2].Equals("double", StringComparison.CurrentCultureIgnoreCase)) {
                                this.AltimeterAltitude = double.Parse(dataVector[1]);
                            }

                            break;
                        case FlightGearInput.Longitude:
                            if (dataVector[2].Equals("double", StringComparison.CurrentCultureIgnoreCase)) {
                                this.Longitude = double.Parse(dataVector[1]);
                            }

                            break;
                        case FlightGearInput.Latitude:
                            if (dataVector[2].Equals("double", StringComparison.CurrentCultureIgnoreCase)) {
                                this.Latitude = double.Parse(dataVector[1]);
                            }

                            break;
                        default:
                            break;
                    }
                } catch (IOException e) {
                    this.Disconnect();
                } catch (Exception e) {
                    Console.WriteLine(e);
                    continue;
                }
            }
        }

        private static FlightGearInput stringToEnum(string property) {
            FlightGearInput enumProperty = (FlightGearInput)Enum.Parse(typeof(FlightGearInput), property, true);
            return enumProperty;
        }

        public void NotifyPropertyChanged(string propName) {
            if (this.PropertyChanged != null) {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        private void NotifyError() {
            string error = string.Empty;
            if (this.latitudeError) {
                error = "latitude is out of boundaries";
            }

            if (this.longitudeError) {
                if (error.Length != 0) {
                    error = "latitude and longitude are out of boundaries";
                } else {
                    error = "longitude is out of boundaries";
                }
            }

            this.ErrorBoundaries = error;
        }

        public bool Running {
            get { return this.running; }
        }
        public double Heading {
            get => this.heading;

            set {
                this.heading = value;
                this.NotifyPropertyChanged("Heading");
            }
        }

        public double VerticalSpeed {
            get => this.verticalSpeed;

            set {
                this.verticalSpeed = value;
                this.NotifyPropertyChanged("VerticalSpeed");
            }
        }

        public double AirSpeed {
            get => this.airSpeed;

            set {
                this.airSpeed = value;
                this.NotifyPropertyChanged("AirSpeed");
            }
        }

        public double GroundSpeed {
            get => this.groundSpeed;

            set {
                this.groundSpeed = value;
                this.NotifyPropertyChanged("GroundSpeed");
            }
        }

        public double GpsAltitude {
            get => this.gpsAltitude;

            set {
                this.gpsAltitude = value;
                this.NotifyPropertyChanged("GpsAltitude");
            }
        }

        public double InternalRoll {
            get => this.internalRoll;

            set {
                this.internalRoll = value;
                this.NotifyPropertyChanged("InternalRoll");
            }
        }

        public double InternalPitch {
            get => this.internalPitch;

            set {
                this.internalPitch = value;
                this.NotifyPropertyChanged("InternalPitch");
            }
        }

        public double AltimeterAltitude {
            get => this.altimeterAltitude;

            set {
                this.altimeterAltitude = value;
                this.NotifyPropertyChanged("AltimeterAltitude");
            }
        }

        public double Longitude {
            get => this.longitude;

            set {
                if (Math.Abs(this.longitude - value) > TOLERANCE) {
                    if (value <= MAX_LONGITUDE && value >= MIN_LONGITUDE) {
                        this.longitude = value;
                        this.ErrorBoundaries = string.Empty;
                        this.longitudeError = false;
                    } else {
                        this.longitudeError = true;
                        if (value < MIN_LONGITUDE) {
                            this.longitude = MIN_LONGITUDE;
                        } else {
                            this.longitude = MAX_LONGITUDE;
                        }
                    }
                }

                this.NotifyPropertyChanged("Longitude");
                this.NotifyError();
            }
        }

        public double Latitude {
            get => this.latitude;

            set {
                if (Math.Abs(this.latitude - value) > TOLERANCE) {
                    if (value <= MAX_LATITUDE && value >= MIN_LATITUDE) {
                        this.latitude = value;
                        this.ErrorBoundaries = string.Empty;
                        this.latitudeError = false;
                    } else {
                        this.latitudeError = true;
                        if (value < MIN_LATITUDE) {
                            this.latitude = MIN_LATITUDE;
                        } else {
                            this.latitude = MAX_LATITUDE;
                        }
                    }
                }

                this.NotifyPropertyChanged("Latitude");
                this.NotifyError();
            }
        }

        public string ErrorBoundaries {
            get => this.errorBoundaries;

            set {
                if (this.errorBoundaries != value) {
                    this.errorBoundaries = value;
                    this.NotifyPropertyChanged("ErrorBoundaries");
                }
            }
        }

        public double Throttle {
            get => this.throttle;

            set {
                if (Math.Abs(this.throttle - value) > TOLERANCE) {
                    this.throttle = value;
                    this.tcpHandler.setParameterValue(FlightGearOutput.Throttle, value);
                }
            }
        }
        public double Rudder {
            get => this.rudder;

            set {
                if (Math.Abs(this.rudder - value) > TOLERANCE) {
                    this.rudder = value;
                    this.tcpHandler.setParameterValue(FlightGearOutput.Rudder, value);
                }
            }
        }
        public double Elevator {
            get => this.elevator;

            set {
                if (Math.Abs(this.elevator - value) > TOLERANCE) {
                    this.elevator = value;
                    this.tcpHandler.setParameterValue(FlightGearOutput.Elevator, value);
                }
            }
        }
        public double Aileron {
            get => this.aileron;

            set {
                if (Math.Abs(this.aileron - value) > TOLERANCE) {
                    this.aileron = value;
                    this.tcpHandler.setParameterValue(FlightGearOutput.Aileron, value);
                }
            }
        }

        public Status ConnectionStatus {
            get => this.connectionStatus;
            set {
                if (this.connectionStatus != value) {
                    this.connectionStatus = value;
                    switch (value) {
                        case Status.connect:
                            this.Start();
                            break;
                        case Status.disconnect:
                            this.Disconnect();
                            break;
                    }
                    this.NotifyPropertyChanged("ConnectionStatus");
                }
            }
        }

        public string IpAddress {
            get => this.ipAddress;
            set { 
                this.ipAddress = value;
                NotifyPropertyChanged("IpAddress");
            }
        }
        public int Port {
            get => this.port;
            set {
                this.port = value;
                NotifyPropertyChanged("Port");
            }

        }
    }
}