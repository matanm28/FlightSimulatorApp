using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FlightSimulatorApp.Model;

namespace FlightSimulatorApp {
    class Class1
    {
        public static void test() {
            ITelnetClient server = new TelnetClient();
            server.connect("127.0.0.1", 5402);
            string data = server.read();
            server.send("get position/latitude-deg");
            server.send("get /instrumentation/heading-indicator/indicated-heading-deg");
            data = server.read();

        }

    }
}