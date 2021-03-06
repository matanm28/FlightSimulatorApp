﻿using System;
using System.Text;

namespace FlightSimulatorApp.Model {
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    /// <summary>an implementation for a ITelnetClient.</summary>
    /// <seealso cref="FlightSimulatorApp.Model.ITelnetClient" />
    public class TelnetClient : ITelnetClient {
        /// <summary>The size</summary>
        private const short Size = 512;

        /// <summary>The client</summary>
        private TcpClient client;

        /// <summary>The buffer</summary>
        private string buffer;

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

        /// <summary>Reads this instance.</summary>
        /// <returns>the entire buffer as string</returns>
        public async Task<string> Read() {
            NetworkStream ns = this.client.GetStream();
            byte[] dataBytes = new byte[Size];
            string dataToSend = string.Empty;
            while (!this.buffer.Contains("\r\n/>")) {
                int bytesRead = await ns.ReadAsync(dataBytes, 0, Size).ConfigureAwait(false);
                this.buffer += Encoding.ASCII.GetString(dataBytes, 0, bytesRead);
            }

            int index = this.buffer.IndexOf('>');
            if (index != -1) {
                dataToSend = this.buffer.Substring(0, index);
                dataToSend = this.getValue(dataToSend);
                this.buffer = this.buffer.Remove(0, index + 2);
            }

            return dataToSend;
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
        /// Flushes this instance's buffer.
        /// </summary>
        public void Flush() {
            this.client.GetStream().Flush();
        }

        public string getValue(string input) {
            // dataToSend = dataToSend.Substring(dataToSend.IndexOf('\'') + 1,dataToSend.IndexOf('\'', dataToSend.IndexOf('\'') + 1) - dataToSend.IndexOf('\'') - 1);
            int startIndex = input.IndexOf('\'') + 1;
            int lenght = input.IndexOf('\'', startIndex) - startIndex;
            return input.Substring(startIndex, lenght);
        }
        /// <summary>
        /// Gets or sets the delimiter.
        /// </summary>
        /// <value>
        /// The delimiter.
        /// </value>
        public string Delimiter {
            get { return "\r\n>"; }
        }
    }
}