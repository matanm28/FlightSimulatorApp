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

    class Model : IModel
    {
        private enum FlightGear
        {
            Heading, VerticalSpeed, GroundSpeed,
            AirSpeed, GpsAltitude, InternalRoll,
            InternalPitch, AltimeterAltitude
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
                delegate ()
                    {
                        if (!this.client.IsConnected())
                        {
                            MessageBox.Show("No connection to host", "Error");
                        }
                        while (!this.stop)
                        {
                            this.client.Send("get " + this.paramPathList[(int)FlightGear.Heading]);
                            this.Heading = double.Parse(this.client.Read());



                            Thread.Sleep(250);
                        }
                    }).Start();
        }

        private void intializeParamPathList()
        {
            this.paramPathList = new List<string>();
            this.paramPathList.Insert((int)FlightGear.Heading, string.Empty);
            this.paramPathList.Insert((int)FlightGear.VerticalSpeed, string.Empty);
            this.paramPathList.Insert((int)FlightGear.GroundSpeed, string.Empty);
            this.paramPathList.Insert((int)FlightGear.AirSpeed, string.Empty);
            this.paramPathList.Insert((int)FlightGear.GpsAltitude, string.Empty);
            this.paramPathList.Insert((int)FlightGear.InternalRoll, string.Empty);
            this.paramPathList.Insert((int)FlightGear.InternalPitch, string.Empty);
            this.paramPathList.Insert((int)FlightGear.AltimeterAltitude, string.Empty);
        }
        private void intializePropList()
        {
            this.propList = new List<string>();
            this.paramPathList.Insert((int)FlightGear.Heading, "Heading");
            this.paramPathList.Insert((int)FlightGear.VerticalSpeed, "VerticalSpeed");
            this.paramPathList.Insert((int)FlightGear.GroundSpeed, "GroundSpeed");
            this.paramPathList.Insert((int)FlightGear.AirSpeed, "AirSpeed");
            this.paramPathList.Insert((int)FlightGear.GpsAltitude, "GpsAltitude");
            this.paramPathList.Insert((int)FlightGear.InternalRoll, "InternalRoll");
            this.paramPathList.Insert((int)FlightGear.InternalPitch, "InternalPitch");
            this.paramPathList.Insert((int)FlightGear.AltimeterAltitude, "AltimeterAltitude");
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
            get
            {
                return this.heading;
            }

            set
            {
                this.heading = value;
                this.NotifyPropertyChanged(this.propList[(int)FlightGear.Heading]);

            }
        }

        public double VerticalSpeed
        {
            get
            {
                return this.verticalSpeed;
            }

            set
            {
                this.verticalSpeed = value;
                this.NotifyPropertyChanged(this.propList[(int)FlightGear.VerticalSpeed]);

            }
        }
        public double AirSpeed
        {
            get
            {
                return this.airSpeed;
            }

            set
            {
                this.airSpeed = value;
                this.NotifyPropertyChanged(this.propList[(int)FlightGear.AirSpeed]);

            }
        }
        public double GroundSpeed
        {
            get
            {
                return this.groundSpeed;
            }

            set
            {
                this.groundSpeed = value;
                this.NotifyPropertyChanged(this.propList[(int)FlightGear.GroundSpeed]);

            }
        }
        public double GpsAltitude
        {
            get
            {
                return this.gpsAltitude;
            }

            set
            {
                this.gpsAltitude = value;
                this.NotifyPropertyChanged(this.propList[(int)FlightGear.GpsAltitude]);

            }
        }
        public double InternalRoll
        {
            get
            {
                return this.internalRoll;
            }

            set
            {
                this.internalRoll = value;
                this.NotifyPropertyChanged(this.propList[(int)FlightGear.InternalRoll]);

            }
        }
        public double InternalPitch
        {
            get
            {
                return this.internalPitch;
            }

            set
            {
                this.internalPitch = value;
                this.NotifyPropertyChanged(this.propList[(int)FlightGear.InternalPitch]);

            }
        }
        public double AltimeterAltitude
        {
            get
            {
                return this.altimeterAltitude;
            }

            set
            {
                this.altimeterAltitude = value;
                this.NotifyPropertyChanged(this.propList[(int)FlightGear.AltimeterAltitude]);

            }
        }
    }
}
