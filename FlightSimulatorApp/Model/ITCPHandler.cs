using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.Model {
    using Properties = FlightGearTCPHandler.FG_OutputProperties;

    public delegate void OnDisconnectEventHandler(string error);
    public interface ITCPHandler {
        event OnDisconnectEventHandler DisconnectOccured;
        void connect(string ip, int port);

        void Disconnect();
        void start();

        void stop();

        void setParameterValue(Properties param, double value);
        IList<string> read();

        bool IsConnected { get; }
    }
}
