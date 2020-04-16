namespace FlightSimulatorApp.Model {
    using System.ComponentModel;
    using Status = Controls.ConnectionControl.Status;

    /// <summary>
    /// an interface for Flight Gear simulator model
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public interface IFlightSimulatorModel : INotifyPropertyChanged {
        /// <summary>
        /// Connects the specified IP at the specified port
        /// </summary>
        /// <param name="ip">The IP.</param>
        /// <param name="port">The port.</param>
        /// <returns></returns>
        bool Connect(string ip, int port);

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Starts this instance.
        /// </summary>
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
