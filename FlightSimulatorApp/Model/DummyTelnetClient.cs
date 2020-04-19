using System;
using System.Text;

namespace FlightSimulatorApp.Model {
    using System.Diagnostics;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    class DummyTelnetClient : TelnetClientV2 {
        /// <summary>
        /// Initializes a new instance of the <see cref="DummyTelnetClient"/> class.
        /// </summary>
        public DummyTelnetClient()
            : base() {
        }

        /// <summary>
        /// Reads the data.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        public override async Task<string> Read() {
            string dataToSend = string.Empty;
            if (this.IsConnected()) {
                NetworkStream ns = this.client.GetStream();
                try {
                    byte[] dataBytes = new byte[Size];
                    Task<int> readTask = ns.ReadAsync(dataBytes, 0, Size);
                    Task delayTask = Task.Delay(ns.ReadTimeout);
                    Task task = await Task.WhenAny(readTask, delayTask).ConfigureAwait(false);
                    if (task == readTask) {
                        int bytesRead = await readTask.ConfigureAwait(false);
                        dataToSend = Encoding.ASCII.GetString(dataBytes, 0, bytesRead);
                        return dataToSend;
                    } else {
                        throw new TimeoutException();
                    }
                }
                catch (TimeoutException e) {
                    Debug.WriteLine(e);
                    throw;
                }
                
                catch (Exception e) {
                    Debug.WriteLine(e);
                    throw new IOException(e.Message);
                }
            }

            return dataToSend;
        }

        /// <summary>
        /// Connects the client.
        /// </summary>
        /// <param name="ip">The IP.</param>
        /// <param name="port">The port.</param>
        public override void Connect(string ip, int port) {
            base.Connect(ip, port);
            NetworkStream ns = this.client.GetStream();
            ns.ReadTimeout = 1000 * 10;
        }
    }
}