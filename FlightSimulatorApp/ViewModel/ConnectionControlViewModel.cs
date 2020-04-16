namespace FlightSimulatorApp.ViewModel {
    using FlightSimulatorApp.Controls;
    using FlightSimulatorApp.Model;
    using Status = Controls.ConnectionControl.Status;
    /// <summary>
    /// a concrete view model class who connects the model to the <see cref="ConnectionControl"/>
    /// </summary>
    /// <seealso cref="FlightSimulatorApp.ViewModel.AFlightGearViewModel" />
    class ConnectionControlViewModel : AFlightGearViewModel {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionControlViewModel"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
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
