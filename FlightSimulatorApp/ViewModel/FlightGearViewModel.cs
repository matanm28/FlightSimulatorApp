using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.ViewModel {
    using System.ComponentModel;
    using FlightSimulatorApp.Model;

    public class FlightGearViewModel : INotifyPropertyChanged {
        private IModel model;
        public event PropertyChangedEventHandler PropertyChanged;

        // Properties
        public double VM_Heading {
            get { return this.model.Heading; }
        }
        public double VM_VerticalSpeed {
            get { return this.model.VerticalSpeed; }
        }
        public double VM_AirSpeed {
            get { return this.model.AirSpeed; }
        }
        public double VM_GroundSpeed {
            get { return this.model.GroundSpeed; }
        }
        public double VM_GpsAltitude {
            get { return this.model.GpsAltitude; }
        }
        public double VM_InternalRoll {
            get { return this.model.InternalRoll; }
        }
        public double VM_InternalPitch {
            get { return this.model.InternalPitch; }
        }
        public double VM_AltimeterAltitude {
            get { return this.model.AltimeterAltitude; }
        }
        public double VM_Throttle {
            get { return this.model.Throttle; }
            set { this.model.Throttle = value; }
        }
        public double VM_Rudder {
            get { return this.model.Rudder; }
            set { this.model.Rudder = value; }
        }

        public double VM_Elevator {
            get { return this.model.Elevator; }
            set { this.model.Elevator = value; }
        }
        public double VM_Aileron {
            get { return this.model.Aileron; }
            set { this.model.Aileron = value; }
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object"/> class.</summary>
        /// <param name="model"></param>
        public FlightGearViewModel(IModel model) {
            this.model = model;
            model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e) {
                this.NotifyPropertyChanged("VM_" + e.PropertyName);
            };
            this.VM_Throttle = 0;
        }

        /// <summary>Notifies the property changed.</summary>
        /// <param name="propName">Name of the property.</param>
        public void NotifyPropertyChanged(string propName) {
            if (this.PropertyChanged != null) {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public void Start(string ip, int port) {
            this.model.Connect(ip, port);
            this.model.Start();
        }
    }
}