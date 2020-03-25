using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.ViewModel {
    using System.ComponentModel;
    using FlightSimulatorApp.Model;

    class FlightGearViewModel : INotifyPropertyChanged {
        private IModel model;
        public event PropertyChangedEventHandler PropertyChanged;

        // Properties
        public double VM_Heading {
            get { return this.model.Heading; }
        }
        public double VM_VerticalSpeed {
            get { return this.model.VerticalSpeed; }
        }
        double VM_AirSpeed {
            get { return this.model.AirSpeed; }
        }
        double VM_GroundSpeed {
            get { return this.model.GroundSpeed; }
        }
        double VM_GpsAltitude {
            get { return this.model.GpsAltitude; }
        }
        double VM_InternalRoll {
            get { return this.model.InternalRoll; }
        }
        double VM_InternalPitch {
            get { return this.model.InternalPitch; }
        }
        double VM_AltimeterAltitude {
            get { return this.model.AltimeterAltitude; }
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object"/> class.</summary>
        /// <param name="model"></param>
        public FlightGearViewModel(IModel model) {
            this.model = model;
            model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e) {
                this.NotifyPropertyChanged("VM_" + e.PropertyName);
            };
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