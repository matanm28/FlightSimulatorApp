using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.Model
{
    using System.Collections;
    using System.Net;
    using System.Net.Sockets;
    using System.Windows;

    public class TelnetClient : ITelnetClient
    {
        public const short SIZE = 512;
        private TcpClient client;

        private string buffer;

        public void connect(string ip, int port)
        {
            this.client = new TcpClient(AddressFamily.InterNetwork);
            this.buffer = String.Empty;
            do
            {
                try
                {
                    this.client.Connect(IPAddress.Parse(ip), port);
                }
                catch (SocketException socketException)
                {
                    Console.WriteLine(socketException);
                    continue;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    break;
                }
            }
            while (!this.client.Connected);
        }

        public void disconnect()
        {
            this.client.Close();
        }

        public void send(string data)
        {
            NetworkStream ns = this.client.GetStream();
            byte[] dataBytes = Encoding.ASCII.GetBytes(data);
            ns.Write(dataBytes, 0, dataBytes.Length);
            long me = ns.Length;
            bool flag = ns.DataAvailable;
            ns.Read(dataBytes, 0, dataBytes.Length);
            this.buffer += Encoding.ASCII.GetString(dataBytes);
        }

        public string read(int numOfBytes)
        {
            NetworkStream ns = this.client.GetStream();
            byte[] dataBytes = new byte[SIZE];
            string dataToSend = String.Empty;
            ////buffer contains all data
            if (this.buffer.Length >= numOfBytes)
            {
                dataToSend = this.buffer.Substring(0, numOfBytes);
                this.buffer = this.buffer.Remove(0, numOfBytes);
            }
            else
            {
                ////reads from network stream
                do
                {
                    ns.Read(dataBytes, 0, SIZE);
                    this.buffer += Encoding.ASCII.GetString(dataBytes);
                }
                while (this.buffer.Length < numOfBytes);
                dataToSend = this.buffer.Substring(0, numOfBytes);
                this.buffer = this.buffer.Remove(0, numOfBytes);
            }
            return dataToSend;
        }

        public string read()
        {
            string dataToSend = String.Empty;
            NetworkStream ns = this.client.GetStream();
            byte[] dataBytes = new byte[SIZE];
            while (ns.DataAvailable)
            {
                ns.Read(dataBytes, 0, SIZE);
                this.buffer += Encoding.ASCII.GetString(dataBytes);
            }
            dataToSend = this.buffer.Substring(0, this.buffer.Length);
            this.buffer = String.Empty;
            return dataToSend;
        }
    }
}