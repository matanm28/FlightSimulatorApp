using Microsoft.Maps.MapControl.WPF;
using System;

namespace FlightSimulatorApp.ViewModel {
    using FlightSimulatorApp.Model;
    using System.ComponentModel;
    using modelStatus = Controls.ConnectionControl.Status;
    /// <summary>
    /// all the view models in one instance.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class FlightGearViewModel : INotifyPropertyChanged {
        private IFlightSimulatorModel model;
        public event PropertyChangedEventHandler PropertyChanged;
        private modelStatus running = modelStatus.inActive;
        private Location location;

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
        public modelStatus VM_Status {
            get { return this.running; }
            set {
                this.running = value;
                this.NotifyPropertyChanged("VM_Status");
            }
        }
        public double VM_Latitude {
            get { return this.model.Latitude; }
        }
        public double VM_Longitude {
            get { return this.model.Longitude; }
        }
        public Location VM_Location {
            get { return this.location; }
            set { this.location = value; }
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object"/> class.</summary>
        /// <param name="model"></param>
        public FlightGearViewModel(IFlightSimulatorModel model) {
            this.model = model;
            model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e) {
                this.NotifyPropertyChanged("VM_" + e.PropertyName);
                if (e.PropertyName == "Latitude" || e.PropertyName == "Longitude") {
                    this.VM_Location = new Location(VM_Latitude, VM_Longitude);
                    this.NotifyPropertyChanged("VM_Location");
                }
            };
        }

        /// <summary>Notifies the property changed.</summary>
        /// <param name="propName">Name of the property.</param>
        public void NotifyPropertyChanged(string propName) {
            if (this.PropertyChanged != null) {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        /// <summary>
        /// Start connecting.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        public void Start(string ip, int port) {
            try {
                this.model.Connect(ip, port);
                this.model.Start();
            } catch (Exception e) {
                throw e;
            }
        }
        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop() {
            this.model.Disconnect();
        }
    }
}