using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maps.MapControl.WPF;

namespace FlightSimulatorApp.Model {
    using System.ComponentModel;
    using Status = Controls.ConnectionControl.Status;

    public interface IFlightSimulatorModel : INotifyPropertyChanged {
        bool Connect(string ip, int port);

        void Disconnect();

        void Start();

        //model properties
        bool Running { get; }
        double Heading { get; set; }

        double VerticalSpeed { get; set; }

        double AirSpeed { get; set; }

        double GroundSpeed { get; set; }

        double GpsAltitude { get; set; }

        double InternalRoll { get; set; }

        double InternalPitch { get; set; }

        double AltimeterAltitude { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }

        double Throttle { get; set; }
        double Rudder { get; set; }
        double Elevator { get; set; }
        double Aileron { get; set; }

        string ErrorBoundaries { get; set; }

        Status ConnectionStatus { get; set; }

        string IpAddress { get; set; }

        int Port { get; set; }
    }
}
