using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maps.MapControl.WPF;

namespace FlightSimulatorApp.Model {
    using System.Collections;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using FlightGearInput = FlightGearTCPHandler.FG_InputProperties;
    using FlightGearOutput = FlightGearTCPHandler.FG_OutputProperties;

    public class FlightSimulatorModel : IFlightSimulatorModel {
        
        private FlightGearTCPHandler tcpHandler;
        private const double TOLERANCE = 0.0001;
        private volatile bool stop = false;
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
        public event PropertyChangedEventHandler PropertyChanged;

        public FlightSimulatorModel() {
            this.tcpHandler = new FlightGearTCPHandler(new TelnetClientV2());

        }

        public void Connect(string ip, int port) {
            this.tcpHandler.connect(ip, port);
        }

        public void Disconnect() {
            this.stop = true;
            this.tcpHandler.disconnect();
        }

        public void Start() {
            this.tcpHandler.start();
            Thread runThread = new Thread(this.run);
            runThread.Name = "runThread";
            runThread.Start();
        }

        private void run() {
            while (!this.stop) {
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
                } catch (Exception e) {
                    Console.WriteLine(e);
                    continue;
                    
                }
            }
        }

        private static FlightGearInput stringToEnum(string property) {
            FlightGearInput enumProperty = (FlightGearInput) Enum.Parse(typeof(FlightGearInput), property, true);
            return enumProperty;
        }

        public void NotifyPropertyChanged(string propName) {
            if (this.PropertyChanged != null) {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
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
                    this.longitude = value;
                    this.NotifyPropertyChanged("Longitude");
                }
            }
        }

        public double Latitude {
            get => this.latitude;

            set {
                if (Math.Abs(this.latitude - value) > TOLERANCE) {
                    this.latitude = value;
                    this.NotifyPropertyChanged("Latitude");
                }
            }
        }

        public double Throttle {
            get => this.throttle;

            set {
                if (Math.Abs(this.throttle - value) > TOLERANCE) {
                    this.throttle = value;
                    this.tcpHandler.setParameterValue(FlightGearOutput.Throttle,value);
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
    }
}