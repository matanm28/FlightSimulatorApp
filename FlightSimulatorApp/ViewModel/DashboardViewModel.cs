using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.ViewModel {
    using FlightSimulatorApp.Model;

    class DashboardViewModel : AFlightGearViewModel{
        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardViewModel"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public DashboardViewModel(IFlightSimulatorModel model)
            : base(model) {
        }

        public DashboardViewModel() {
        }

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
    }
}
