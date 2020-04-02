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

        public AFlightGearViewModel() {

        }
        public AFlightGearViewModel(IFlightSimulatorModel model) {
            this.model = model;
            model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e) {
                this.NotifyPropertyChanged("VM_" + e.PropertyName);
            };
        }

        public void NotifyPropertyChanged(string propName) {
            if (this.PropertyChanged != null) {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

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

        public void Stop() {
            if (this.model.Running) {
                this.model.Disconnect();
            }
        }

        public virtual void setModel(IFlightSimulatorModel model) {
            this.model = model;
        }

    }
}
