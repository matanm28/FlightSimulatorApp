using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.Model {
    class FG_Data {
        private string property = null;
        private string value = null;
        private string cast = null;
        private bool isValid = false;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public FG_Data() {
        }
        /// properties.
        public string Property {
            get => this.property;
            set => this.property = value;
        }
        public string Value {
            get => this.value;
            set => this.value = value;
        }
        public string Cast {
            get => this.cast;
            set => this.cast = value;
        }

    }
}
