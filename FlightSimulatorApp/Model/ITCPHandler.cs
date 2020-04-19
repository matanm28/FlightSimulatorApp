using System.Collections.Generic;

namespace FlightSimulatorApp.Model {
    using System.ComponentModel;
    using Properties = FlightGearTCPHandler.FG_OutputProperties;

    /// <summary>
    /// parses and handles data coming via TCP connection for a certain model.
    /// </summary>
    public interface ITCPHandler : INotifyPropertyChanged{
        
        /// <summary>
        /// Connects the specified ip.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        void Connect(string ip, int port);
        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        void Disconnect();
        /// <summary>
        /// Starts this instance.
        /// </summary>
        void Start();
        /// <summary>
        /// Stops this instance.
        /// </summary>
        void Stop();

        /// <summary>
        /// Sets the parameter value.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="value">The value.</param>
        void SetParameterValue(Properties param, double value);
        /// <summary>
        /// Reads this instance.
        /// </summary>
        /// <returns></returns>
        IList<string> Read();

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        bool IsConnected { get; }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        string Error { get; set; }
    }
}
