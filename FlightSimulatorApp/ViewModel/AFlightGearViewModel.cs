using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.ViewModel {
    using System.ComponentModel;
    using FlightSimulatorApp.Model;

    class AFlightGearViewModel : INotifyPropertyChanged {
        protected IFlightSimulatorModel model;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="AFlightGearViewModel"/> class.
        /// </summary>
        public AFlightGearViewModel() {

        }

        /// <summary>Initializes a new instance of the <see cref="AFlightGearViewModel" /> class.</summary>
        /// <param name="model">The model.</param>
        public AFlightGearViewModel(IFlightSimulatorModel model) {
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

        /// <summary>
        /// Start connecting.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        public void Start(string ip, int port) {
            if (!this.model.Running) {
                try {
                    this.model.Connect(ip, port);
                    this.model.Start();
                } catch (Exception e) {
                    throw e;
                }
            }
        }
        /// <summary>
        /// Stops the connection.
        /// </summary>
        public void Stop() {
            if (this.model.Running) {
                this.model.Disconnect();
            }
        }

        /// <summary>
        /// Sets the model.
        /// </summary>
        /// <param name="model">The model.</param>
        public virtual void SetModel(IFlightSimulatorModel model) {
            this.model = model;
        }

    }
}
