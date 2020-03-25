﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.Model
{
    using Microsoft.Win32;

    interface ITelnetClient
    {
        void Connect(string ip, int port);

        bool IsConnected();

        void Disconnect();

        void Send(string data);

        string Read(int numOfBytes);
        string Read();
    }
}