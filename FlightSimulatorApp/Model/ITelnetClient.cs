using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.Model
{
    using Microsoft.Win32;

    interface ITelnetClient
    {
        void connect(string ip, int port);

        void disconnect();

        void send(string data);

        string read(int numOfBytes);
        string read();
    }
}