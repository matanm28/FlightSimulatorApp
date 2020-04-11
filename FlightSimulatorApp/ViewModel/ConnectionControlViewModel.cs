using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.ViewModel {
    using System.ComponentModel;
    using FlightSimulatorApp.Model;
    using Status = Controls.ConnectionControl.Status;
    class ConnectionControlViewModel : AFlightGearViewModel{
        public ConnectionControlViewModel(IFlightSimulatorModel model)
            : base(model) {
        }
        // Properties
        public Status VM_ConnectionStatus {
            get { return this.model.ConnectionStatus; }
            set { this.model.ConnectionStatus = value; }
        }

        public string VM_IpAddress {
            get => this.model.IpAddress;
            set { this.model.IpAddress = value; }
        }
        public int VM_Port {
            get => this.model.Port;
            set => this.model.Port = value;
        }

    }
}
