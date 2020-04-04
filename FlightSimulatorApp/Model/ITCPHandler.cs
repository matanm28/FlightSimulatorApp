using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.Model {
    using Properties = FlightGearTCPHandler.FG_OutputProperties;
    public interface ITCPHandler {

        void connect(string ip, int port);

        void disconnect();
        void start();

        void stop();

        void setParameterValue(Properties param, double value);
        IList<string> read();
    }
}
