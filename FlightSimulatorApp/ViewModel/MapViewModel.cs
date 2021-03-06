﻿namespace FlightSimulatorApp.ViewModel {
    using FlightSimulatorApp.Model;
    using Microsoft.Maps.MapControl.WPF;
    using System.ComponentModel;

    class MapViewModel : AFlightGearViewModel {
        private Location location;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapViewModel"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public MapViewModel(IFlightSimulatorModel model)
            : base(model) {
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
        public override void SetModel(IFlightSimulatorModel model) {
            this.model = model;
            model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e) {
                this.NotifyPropertyChanged("VM_" + e.PropertyName);
                if (e.PropertyName == "Latitude" || e.PropertyName == "Longitude") {
                    this.VM_Location = new Location(VM_Latitude, VM_Longitude);
                    this.NotifyPropertyChanged("VM_Location");
                }
            };
        }
    }
}