using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.Model {
    using System.IO;
    using System.Net.Sockets;

    class DummyTelnetClient : TelnetClientV2 {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public DummyTelnetClient()
            : base() {
        }

        /// <inheritdoc />
        public override string read() {
            string dataToSend = string.Empty;
            if (this.isConnected()) {
                NetworkStream ns = this.client.GetStream();
                try {
                    byte[] dataBytes = new byte[Size];
                        int bytesRead = ns.Read(dataBytes, 0, Size);
                        dataToSend = Encoding.ASCII.GetString(dataBytes, 0, bytesRead);
                        return dataToSend;
                } catch (Exception e) {
                    throw new IOException();
                }
            }
            return dataToSend;
        }
    }
}