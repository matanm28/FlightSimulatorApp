using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.Model {
    using System.Collections;
    using System.Net;
    using System.Net.Sockets;
    using System.Windows;

    /// <summary>an implementation for a ITelnetClient.</summary>
    /// <seealso cref="FlightSimulatorApp.Model.ITelnetClient" />
    public class TelnetClient : ITelnetClient {
        /// <summary>The size</summary>
        private const short Size = 512;

        /// <summary>The client</summary>
        private TcpClient client;

        /// <summary>The buffer</summary>
        private String buffer;

        /// <summary>Connects the specified IP.</summary>
        /// <param name="ip">The IP.</param>
        /// <param name="port">The port.</param>
        public void Connect(string ip, int port) {
            this.client = new TcpClient(AddressFamily.InterNetwork);

            this.buffer = string.Empty;
            do {
                try {
                    this.client.Connect(IPAddress.Parse(ip), port);
                } catch (SocketException socketException) {
                    Console.WriteLine(socketException);
                    continue;
                } catch (Exception e) {
                    Console.WriteLine(e);
                    break;
                }
            }
            while (!this.client.Connected);
        }

        /// <summary>Disconnects this instance.</summary>
        public void Disconnect() {
            this.client.Close();
        }

        /// <summary>Sends the specified data.</summary>
        /// <param name="data">The data.</param>
        public void Send(string data) {
            NetworkStream ns = this.client.GetStream();
            byte[] dataBytes = Encoding.ASCII.GetBytes(data);
            ns.Write(dataBytes, 0, dataBytes.Length);
        }

        /// <summary>Reads the specified number of bytes.</summary>
        /// <param name="numOfBytes">The number of bytes to read from buffer.</param>
        /// <returns> a data string of length <param name="numOfBytes"></param> from the buffer</returns>
        public string Read(int numOfBytes) {
            NetworkStream ns = this.client.GetStream();
            byte[] dataBytes = new byte[Size];
            string dataToSend = string.Empty;
            ////buffer contains all data
            if (this.buffer.Length >= numOfBytes) {
                dataToSend = this.buffer.Substring(0, numOfBytes);
                this.buffer = this.buffer.Remove(0, numOfBytes);
            } else {
                ////reads from network stream
                do {
                    ns.Read(dataBytes, 0, Size);
                    this.buffer += Encoding.ASCII.GetString(dataBytes);
                }
                while (this.buffer.Length < numOfBytes);
                dataToSend = this.buffer.Substring(0, numOfBytes);
                this.buffer = this.buffer.Remove(0, numOfBytes);
            }
            return dataToSend;
        }

        /// <summary>Reads this instance.</summary>
        /// <returns>the entire buffer as string</returns>
        public string Read() {
            NetworkStream ns = this.client.GetStream();
            byte[] dataBytes = new byte[Size];
            string dataToSend = string.Empty;
            while (!this.buffer.Contains("\r\n/>")) {
                int bytesRead = ns.Read(dataBytes, 0, Size);
                this.buffer += Encoding.ASCII.GetString(dataBytes, 0, bytesRead);
            }
            int index = this.buffer.IndexOf('>');
            if (index != -1) {
                dataToSend = this.buffer.Substring(0, index);
                dataToSend = dataToSend.Substring(
                    dataToSend.IndexOf('\'') + 1,
                    dataToSend.IndexOf('\'', dataToSend.IndexOf('\'') + 1) - dataToSend.IndexOf('\'') - 1);
                this.buffer = this.buffer.Remove(0, index + 2);
            }
            return dataToSend;
        }
        public bool IsConnected() {
            return this.client.Connected;
        }
    }
}