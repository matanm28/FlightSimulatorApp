﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightSimulatorApp.Model {
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using FlightGearInput = FlightGearTCPHandler.FG_InputProperties;
    using FlightGearOutput = FlightGearTCPHandler.FG_OutputProperties;
    using Status = Controls.ConnectionControl.Status;

    /// <summary>
    /// handles the incoming and outgoing data from Flight Gear Simulator.
    /// </summary>
    /// <seealso cref="FlightSimulatorApp.Model.IFlightSimulatorModel" />
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

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightSimulatorModel"/> class.
        /// </summary>
        public FlightSimulatorModel() {
            this.tcpHandler = new FlightGearTCPHandler(new TelnetClientV2());
            this.tcpHandler.PropertyChanged += this.PropertyChangedHandler;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightSimulatorModel"/> class.
        /// </summary>
        /// <param name="tcpHandler">The TCP handler.</param>
        public FlightSimulatorModel(ITCPHandler tcpHandler) {
            this.tcpHandler = tcpHandler;
            this.tcpHandler.PropertyChanged += this.PropertyChangedHandler;
        }

        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(this.tcpHandler.Error):
                    this.Error = this.tcpHandler.Error;
                    break;
                case nameof(this.tcpHandler.IsConnected):
                    if (this.tcpHandler.IsConnected) {
                        this.connectionStatus = Status.active;
                    } else {
                        this.ConnectionStatus = Status.disconnect;
                    }
                    break;
            }
        }

        /// <summary>Connects the specified ip.</summary>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        /// <exception cref="TimeoutException">if model wasn't able to Connect</exception>
        public bool Connect(string ip, int port) {
            this.tcpHandler.Connect(ip, port);
            if (this.tcpHandler.IsConnected) {
                this.resendSetValues();
            }

            return this.tcpHandler.IsConnected;
        }

        /// <summary>
        /// Resends the set values.
        /// </summary>
        private void resendSetValues() {
            this.tcpHandler.SetParameterValue(FlightGearOutput.Throttle, this.Throttle);
            this.tcpHandler.SetParameterValue(FlightGearOutput.Rudder, this.Rudder);
            this.tcpHandler.SetParameterValue(FlightGearOutput.Aileron, this.Aileron);
            this.tcpHandler.SetParameterValue(FlightGearOutput.Elevator, this.Elevator);
        }

        /// <summary>
        /// Disconnects TcpHandler.
        /// </summary>
        public async void Disconnect() {
            if (this.connectionStatus != Status.inActive) {
                this.running = false;
                await Task.Run(() => this.tcpHandler.Disconnect());
                this.ConnectionStatus = Status.inActive;
            }
        }

        /// <summary>
        /// Starts the TcpHandler.
        /// </summary>
        public async void Start() {
            bool flag = false;
            if (!this.tcpHandler.IsConnected) {
                flag = await Task<bool>.Run(() => this.Connect(this.IpAddress, this.Port));
            }

            if (flag) {
                this.running = true;
                await Task.Run(() => this.tcpHandler.Start());
                Thread runThread = new Thread(this.run);
                runThread.Name = "runThread";
                runThread.Start();
                this.ConnectionStatus = Status.active;
            } else {
                this.ConnectionStatus = Status.inActive;
            }
        }

        /// <summary>
        /// Reads the data, gets rhe propertyName and then parsing.
        /// </summary>
        private void run() {
            while (this.running) {
                try {
                    IList<string> dataVector = this.tcpHandler.Read();
                    if (dataVector == null) {
                        return;
                    }
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
                }
                catch (IOException e) {
                    Console.WriteLine(e);
                    this.Disconnect();
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                    continue;
                }
            }
        }

        public static FlightGearInput stringToEnum(string property) {
            FlightGearInput enumProperty = (FlightGearInput)Enum.Parse(typeof(FlightGearInput), property, true);
            return enumProperty;
        }

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propName">Name of the property.</param>
        public void NotifyPropertyChanged(string propName) {
            if (this.PropertyChanged != null) {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        /// <summary>
        /// Notifies the error.
        /// </summary>
        private void notifyError() {
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

        // properties:
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
                this.notifyError();
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
                this.notifyError();
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
                    this.tcpHandler.SetParameterValue(FlightGearOutput.Throttle, value);
                }
            }
        }
        public double Rudder {
            get => this.rudder;

            set {
                if (Math.Abs(this.rudder - value) > TOLERANCE) {
                    this.rudder = value;
                    this.tcpHandler.SetParameterValue(FlightGearOutput.Rudder, value);
                }
            }
        }
        public double Elevator {
            get => this.elevator;

            set {
                if (Math.Abs(this.elevator - value) > TOLERANCE) {
                    this.elevator = value;
                    this.tcpHandler.SetParameterValue(FlightGearOutput.Elevator, value);
                }
            }
        }
        public double Aileron {
            get => this.aileron;

            set {
                if (Math.Abs(this.aileron - value) > TOLERANCE) {
                    this.aileron = value;
                    this.tcpHandler.SetParameterValue(FlightGearOutput.Aileron, value);
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
        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public string Error {
            get {
                return this.tcpHandler.Error;
            }
            set {
                if (value != null) {
                    this.tcpHandler.Error = value;
                    this.NotifyPropertyChanged(nameof(this.Error));
                }
            }
        }
    }
}