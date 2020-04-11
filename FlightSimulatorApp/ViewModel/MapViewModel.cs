using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.ViewModel {
    using System.ComponentModel;
    using FlightSimulatorApp.Model;
    using Microsoft.Maps.MapControl.WPF;

    class MapViewModel : AFlightGearViewModel {
        private Location location;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapViewModel"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public MapViewModel(IFlightSimulatorModel model)
            : base() {
            this.model = model;
            this.model.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e) {
                this.NotifyPropertyChanged("VM_" + e.PropertyName);
                this.VM_Location = new Location(VM_Latitude, VM_Longitude);
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapViewModel"/> class.
        /// </summary>
        public MapViewModel() {
        }

        // properties
        public double VM_Latitude {
            get { return this.model.Latitude; }
        }

        public double VM_Longitude {
            get { return this.model.Longitude; }
        }

        public string VM_ErrorBoundaries {
            get { return $"{this.model.ErrorBoundaries}"; }
        }

        public Location VM_Location {
            get { return this.location; }
            set {
                this.location = value;
                this.NotifyPropertyChanged("VM_Location");
            }
        }

        /// <summary>
        /// Sets the model.
        /// </summary>
        /// <param name="model">The model.</param>
/        public override void SetModel(IFlightSimulatorModel model) {
            base.SetModel(model);
            model.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e) {
                this.NotifyPropertyChanged("VM_" + e.PropertyName);
                if (e.PropertyName == "Latitude" || e.PropertyName == "Longitude") {
                    this.VM_Location = new Location(VM_Latitude, VM_Longitude);
                    this.NotifyPropertyChanged("VM_Location");
                }
            };
        }
    }
}