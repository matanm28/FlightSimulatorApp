using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.Model {
    using System.IO;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;

    class TelnetClientV2 : ITelnetClient {
        /// <summary>The size</summary>
        private const short Size = 512;

        /// <summary>The client</summary>
        private TcpClient client;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TelnetClientV2() {
            this.client = new TcpClient(AddressFamily.InterNetwork);
        }

        public void connect(string ip, int port) {
            this.client.Connect(IPAddress.Parse(ip), port);
        }

        public bool isConnected() {
            return this.client.Connected;
        }

        public void disconnect() {
            this.client.Close();
            this.client = new TcpClient(AddressFamily.InterNetwork);
        }

        public void send(string data) {
            //todo wrap try catch and disconnect
            try {
                if (this.isConnected()) {
                    NetworkStream networkStream = this.client.GetStream();
                    byte[] sendBytes = Encoding.ASCII.GetBytes(data);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                }
            } catch (Exception e) {
                throw new IOException();
            }
        }

        public void flush() {
            this.client.GetStream().Flush();
        }
        public string read() {
            string dataToSend = string.Empty;
            if (this.isConnected()) {
                NetworkStream ns = this.client.GetStream();
                try {
                    if (ns.DataAvailable) {
                        byte[] dataBytes = new byte[Size];
                        int bytesRead = ns.Read(dataBytes, 0, Size);
                        dataToSend = Encoding.ASCII.GetString(dataBytes, 0, bytesRead);
                        return dataToSend;
                    }
                } catch (Exception e) {
                    throw new IOException();
                }
            } 

            return dataToSend;

        }
    }
}