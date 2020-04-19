using System;
using System.Text;

namespace FlightSimulatorApp.Model {
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    class TelnetClientV2 : ITelnetClient {
        /// <summary>The size</summary>
        protected const short Size = 512;

        /// <summary>The client</summary>
        protected TcpClient client;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TelnetClientV2() {
            this.client = new TcpClient(AddressFamily.InterNetwork);
        }

        /// <summary>
        /// Connects the client.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        public virtual void Connect(string ip, int port) {
            this.client.Connect(IPAddress.Parse(ip), port);
        }

        /// <summary>
        /// Determines whether this instance is connected.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </returns>
        public bool IsConnected() {
            return this.client.Connected;
        }

        /// <summary>
        /// Disconnects the client.
        /// </summary>
        public void Disconnect() {
            if (this.client.Connected) {
                this.client.Close();
            }
            this.client = new TcpClient(AddressFamily.InterNetwork);
        }

        /// <summary>
        /// Sends the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="IOException"></exception>
        public void Send(string data) {
            try {
                if (this.IsConnected()) {
                    NetworkStream networkStream = this.client.GetStream();
                    byte[] sendBytes = Encoding.ASCII.GetBytes(data);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                }
            }
            catch (Exception e) {
                throw new IOException(e.Message);
            }
        }

        /// <summary>
        /// Flushes the stream.
        /// </summary>
        public void Flush() {
            this.client.GetStream().Flush();
        }

        /// <summary>
        /// Reads the data.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        public virtual async Task<string> Read() {
            string dataToSend = string.Empty;
            if (this.IsConnected()) {
                NetworkStream ns = this.client.GetStream();
                try {
                    if (ns.DataAvailable) {
                        byte[] dataBytes = new byte[Size];
                        int bytesRead = await ns.ReadAsync(dataBytes, 0, Size).ConfigureAwait(false);
                        dataToSend = Encoding.ASCII.GetString(dataBytes, 0, bytesRead);
                        return dataToSend;
                    }
                }
                catch (Exception e) {
                    throw new IOException(e.Message);
                }
            }

            return dataToSend;
        }
    }
}