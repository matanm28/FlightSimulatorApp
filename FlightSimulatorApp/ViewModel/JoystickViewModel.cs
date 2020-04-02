using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.ViewModel {
    using FlightSimulatorApp.Model;

    class JoystickViewModel : AFlightGearViewModel{
        public JoystickViewModel(IFlightSimulatorModel model)
            : base(model) {
        }

        public JoystickViewModel() {
        }

        //properties
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
    }
}
