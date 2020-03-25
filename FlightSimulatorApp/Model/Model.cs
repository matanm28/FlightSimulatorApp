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

        private IList<string> propList;

        private volatile bool stop;
        private double heading;
        private double verticalSpeed;
        private double groundSpeed;
        private double airSpeed;
        private double gpsAltitude;
        private double internalRoll;
        private double internalPitch;
        private double altimeterAltitude;

        public event PropertyChangedEventHandler PropertyChanged;

        public Model()
        {
            this.client = new TelnetClient();
            this.stop = false;
            this.intializeParamPathList();
            this.intializePropList();
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
        }

        private void intializePropList()
        {
            this.propList = new List<string>();
            this.propList.Insert((int) FG_Properties.Heading, "Heading");
            this.propList.Insert((int) FG_Properties.VerticalSpeed, "VerticalSpeed");
            this.propList.Insert((int) FG_Properties.GroundSpeed, "GroundSpeed");
            this.propList.Insert((int) FG_Properties.AirSpeed, "AirSpeed");
            this.propList.Insert((int) FG_Properties.GpsAltitude, "GpsAltitude");
            this.propList.Insert((int) FG_Properties.InternalRoll, "InternalRoll");
            this.propList.Insert((int) FG_Properties.InternalPitch, "InternalPitch");
            this.propList.Insert((int) FG_Properties.AltimeterAltitude, "AltimeterAltitude");
        }

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public double Heading
        {
            get => this.heading;

            set
            {
                this.heading = value;
                this.NotifyPropertyChanged(this.propList[(int) FG_Properties.Heading]);
            }
        }

        public double VerticalSpeed
        {
            get => this.verticalSpeed;

            set
            {
                this.verticalSpeed = value;
                this.NotifyPropertyChanged(this.propList[(int) FG_Properties.VerticalSpeed]);
            }
        }

        public double AirSpeed
        {
            get => this.airSpeed;

            set
            {
                this.airSpeed = value;
                this.NotifyPropertyChanged(this.propList[(int) FG_Properties.AirSpeed]);
            }
        }

        public double GroundSpeed
        {
            get => this.groundSpeed;

            set
            {
                this.groundSpeed = value;
                this.NotifyPropertyChanged(this.propList[(int) FG_Properties.GroundSpeed]);
            }
        }

        public double GpsAltitude
        {
            get => this.gpsAltitude;

            set
            {
                this.gpsAltitude = value;
                this.NotifyPropertyChanged(this.propList[(int) FG_Properties.GpsAltitude]);
            }
        }

        public double InternalRoll
        {
            get => this.internalRoll;

            set
            {
                this.internalRoll = value;
                this.NotifyPropertyChanged(this.propList[(int) FG_Properties.InternalRoll]);
            }
        }

        public double InternalPitch
        {
            get => this.internalPitch;

            set
            {
                this.internalPitch = value;
                this.NotifyPropertyChanged(this.propList[(int) FG_Properties.InternalPitch]);
            }
        }

        public double AltimeterAltitude
        {
            get => this.altimeterAltitude;

            set
            {
                this.altimeterAltitude = value;
                this.NotifyPropertyChanged(this.propList[(int) FG_Properties.AltimeterAltitude]);
            }
        }
    }
}