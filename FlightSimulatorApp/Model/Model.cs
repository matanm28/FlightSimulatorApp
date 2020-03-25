using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.Model {
    using System.Collections;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows;

    public class Model : IModel {
        private enum FG_Properties {
            Heading = 0,
            VerticalSpeed = 1,
            GroundSpeed = 2,
            AirSpeed = 3,
            GpsAltitude = 4,
            InternalRoll = 5,
            InternalPitch = 6,
            AltimeterAltitude = 7
        }

        private ITelnetClient client;

        private IList<string> paramPathList;

        private volatile bool stop;
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

        public event PropertyChangedEventHandler PropertyChanged;

        public Model() {
            this.client = new TelnetClient();
            this.stop = false;
            this.intializeParamPathList();
        }

        public void Connect(string ip, int port) {
            this.client.Connect(ip, port);
        }

        public void Disconnect() {
            this.stop = false;
            this.client.Disconnect();
        }

        public void Start() {
            Thread updateThread = new Thread(
                delegate () {
                    if (!this.client.IsConnected()) {
                        MessageBox.Show("No connection to host", "Error");
                    }

                    while (!this.stop) {
                        this.updateData();
                        Thread.Sleep(250);
                    }
                });
            updateThread.Name = "UpdateThread";
            updateThread.Start();
        }

        private void intializeParamPathList() {
            this.paramPathList = new List<string>();
            this.paramPathList.Insert(
                (int)FG_Properties.Heading,
                "/instrumentation/heading-indicator/indicated-heading-deg");
            this.paramPathList.Insert(
                (int)FG_Properties.VerticalSpeed,
                "/instrumentation/gps/indicated-vertical-speed");
            this.paramPathList.Insert((int)FG_Properties.GroundSpeed, "/instrumentation/gps/indicated-ground-speed-kt");
            this.paramPathList.Insert(
                (int)FG_Properties.AirSpeed,
                "/instrumentation/airspeed-indicator/indicated-speed-kt");
            this.paramPathList.Insert((int)FG_Properties.GpsAltitude, "/instrumentation/gps/indicated-altitude-ft");
            this.paramPathList.Insert(
                (int)FG_Properties.InternalRoll,
                "/instrumentation/attitude-indicator/internal-roll-deg");
            this.paramPathList.Insert(
                (int)FG_Properties.InternalPitch,
                "/instrumentation/attitude-indicator/internal-pitch-deg");
            this.paramPathList.Insert(
                (int)FG_Properties.AltimeterAltitude,
                "/instrumentation/altimeter/indicated-altitude-ft");
        }

        private void updateData() {
            this.client.Send("get " + this.paramPathList[(int)FG_Properties.Heading] + " \r\n");
            this.Heading = double.Parse(this.client.Read());
            this.client.Send("get " + this.paramPathList[(int)FG_Properties.VerticalSpeed] + " \r\n");
            this.VerticalSpeed = double.Parse(this.client.Read());
            this.client.Send("get " + this.paramPathList[(int)FG_Properties.GroundSpeed] + " \r\n");
            this.GroundSpeed = double.Parse(this.client.Read());
            this.client.Send("get " + this.paramPathList[(int)FG_Properties.AirSpeed] + " \r\n");
            this.AirSpeed = double.Parse(this.client.Read());
            this.client.Send("get " + this.paramPathList[(int)FG_Properties.GpsAltitude] + " \r\n");
            this.GpsAltitude = double.Parse(this.client.Read());
            this.client.Send("get " + this.paramPathList[(int)FG_Properties.InternalRoll] + " \r\n");
            this.InternalRoll = double.Parse(this.client.Read());
            this.client.Send("get " + this.paramPathList[(int)FG_Properties.InternalPitch] + " \r\n");
            this.InternalPitch = double.Parse(this.client.Read());
            this.client.Send("get " + this.paramPathList[(int)FG_Properties.AltimeterAltitude] + " \r\n");
            this.AltimeterAltitude = double.Parse(this.client.Read());
        }

        public void NotifyPropertyChanged(string propName) {
            if (this.PropertyChanged != null) {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        // todo: check if FG returns after set
        public void moveThrottle(double value) {
            this.client.Send("set /controls/engines/current-engine/throttle " + value.ToString() + " \r\n");
        }

        public void moveRudder(double value) {
            this.client.Send("set /controls/flight/rudder " + value.ToString() + " \r\n");
        }

        public void moveElevator(double value) {
            this.client.Send("set /controls/flight/elevator " + value.ToString() + " \r\n");
        }

        public void moveAileron(double value) {
            this.client.Send("set /controls/flight/aileron " + value.ToString() + " \r\n");
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
        public double Throttle {
            get => this.throttle;

            set {
                if (this.throttle != value) {
                    this.throttle = value;
                    this.moveThrottle(this.throttle);
                }
            }
        }
        public double Rudder {
            get => this.rudder;

            set {
                if (this.rudder != value) {
                    this.rudder = value;
                    this.moveRudder(this.rudder);
                }
            }
        }
        public double Elevator {
            get => this.elevator;

            set {
                if (this.elevator != value) {
                    this.elevator = value;
                    this.moveElevator(this.elevator);
                }
            }
        }
        public double Aileron {
            get => this.aileron;

            set {
                if (this.aileron != value) {
                    this.aileron = value;
                    this.moveAileron(this.aileron);
                }
            }
        }
    }
}