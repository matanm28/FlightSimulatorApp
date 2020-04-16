namespace FlightSimulatorApp.Model {
    public interface ITelnetClient {
        /// <summary>
        /// Connects the specified IP at the specified port.
        /// </summary>
        /// <param name="ip">The IP.</param>
        /// <param name="port">The port.</param>
        void Connect(string ip, int port);

        /// <summary>
        /// Determines whether this instance is connected.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </returns>
        bool IsConnected();

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        void Disconnect();
        /// <summary>
        /// Sends the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        void Send(string data);
        /// <summary>
        /// Reads data from this instance stream.
        /// </summary>
        /// <returns></returns>
        string Read();
        /// <summary>
        /// Flushes this instance's buffer.
        /// </summary>
        void Flush();
    }
}