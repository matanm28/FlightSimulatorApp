using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.Model
{
    using System.Collections;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows;

    internal class Model : IModel
    {
        private enum FG_Properties
        {
            Heading = 0,
            VerticalSpeed,
            GroundSpeed,
            AirSpeed,
            GpsAltitude,
            InternalRoll,
            InternalPitch,
            AltimeterAltitude,
            Longitude
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
        private double longitude;

        public event PropertyChangedEventHandler PropertyChanged;

        public Model()
        {
            this.client = new TelnetClient();
            this.stop = false;
            this.intializeParamPathList();
        }

        public void Connect(string ip, int port)
        {
            this.client.Connect(ip, port);
        }

        public void Disconnect()
        {
            this.stop = false;
            this.client.Disconnect();
        }

        public void Start()
        {
            new Thread(
                delegate()
                {
                    if (!this.client.IsConnected())
                    {
                        MessageBox.Show("No connection to host", "Error");
                    }

                    while (!this.stop)
                    {
                        this.client.Send("get " + this.paramPathList[(int) FG_Properties.Heading] + " \r\n");
                        this.Heading = double.Parse(this.client.Read());

                        Thread.Sleep(250);
                    }
                }).Start();
        }

        private void intializeParamPathList()
        {
            this.paramPathList = new List<string>();
            this.paramPathList.Insert((int) FG_Properties.Heading, "/instrumentation/heading-indicator/indicated-heading-deg");
            this.paramPathList.Insert( (int)FG_Properties.VerticalSpeed, "/instrumentation/gps/indicated-vertical-speed");
            this.paramPathList.Insert((int) FG_Properties.GroundSpeed, "/instrumentation/gps/indicated-ground-speed-kt");
            this.paramPathList.Insert((int) FG_Properties.AirSpeed, "/instrumentation/airspeed-indicator/indicated-speed-kt");
            this.paramPathList.Insert((int) FG_Properties.GpsAltitude, "/instrumentation/gps/indicated-altitude-ft");
            this.paramPathList.Insert((int) FG_Properties.InternalRoll, "/instrumentation/attitude-indicator/internal-roll-deg");
            this.paramPathList.Insert((int) FG_Properties.InternalPitch, "/instrumentation/attitude-indicator/internal-pitch-deg");
            this.paramPathList.Insert((int) FG_Properties.AltimeterAltitude, "/instrumentation/altimeter/indicated-altitude-ft");
            this.paramPathList.Insert((int) FG_Properties.Longitude, "/position/longitude-deg");
        }



        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        //todo: check if FG returns after set
        public void moveThrottle(double value)
        {
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

        public double Heading
        {
            get => this.heading;

            set
            {
                this.heading = value;
                this.NotifyPropertyChanged("Heading");
            }
        }

        public double VerticalSpeed
        {
            get => this.verticalSpeed;

            set
            {
                this.verticalSpeed = value;
                this.NotifyPropertyChanged("VerticalSpeed");
            }
        }

        public double AirSpeed
        {
            get => this.airSpeed;

            set
            {
                this.airSpeed = value;
                this.NotifyPropertyChanged("AirSpeed");
            }
        }

        public double GroundSpeed
        {
            get => this.groundSpeed;

            set
            {
                this.groundSpeed = value;
                this.NotifyPropertyChanged("GroundSpeed");
            }
        }

        public double GpsAltitude
        {
            get => this.gpsAltitude;

            set
            {
                this.gpsAltitude = value;
                this.NotifyPropertyChanged("GpsAltitude");
            }
        }

        public double InternalRoll
        {
            get => this.internalRoll;

            set
            {
                this.internalRoll = value;
                this.NotifyPropertyChanged("InternalRoll");
            }
        }

        public double InternalPitch
        {
            get => this.internalPitch;

            set
            {
                this.internalPitch = value;
                this.NotifyPropertyChanged("InternalPitch");
            }
        }

        public double AltimeterAltitude
        {
            get => this.altimeterAltitude;

            set
            {
                this.altimeterAltitude = value;
                this.NotifyPropertyChanged("AltimeterAltitude");
            }
        }

        public double Longitude {
            get => this.longitude;

            set {
                this.longitude = value;
                this.NotifyPropertyChanged("Longitude");
            }
        }
    }
}