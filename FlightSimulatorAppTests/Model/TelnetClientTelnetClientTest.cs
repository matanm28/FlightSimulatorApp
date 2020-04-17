using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlightSimulatorApp.Model.Tests {
    [TestClass()]
    public class TelnetClientTelnetClientTest {
        [TestMethod()]
        [Timeout(5000)]

        public void ConnectTest() {
            TelnetClient client = new TelnetClient();
            client.Connect("127.0.0.1", 5402);

            Assert.AreEqual(true, client.IsConnected());
        }

        [TestMethod()]

        public async void ReadTest() {
            string expected = "/instrumentation/heading-indicator/indicated-heading-deg =  (double)\r\n/> ";
            string expected1 = "/controls/engines/current-engine/throttle =  (double)\r\n/> ";
            TelnetClient client = new TelnetClient();
            client.Connect("127.0.0.1", 5402);
            client.Send("get /instrumentation/heading-indicator/indicated-heading-deg \r\n");
            string result = await client.Read();
            client.Send("set /controls/engines/current-engine/throttle 1 \r\n");
            string result1 = await client.Read();
            int index = result.IndexOf('\'');
            int end = result.IndexOf('\'', index + 1);
            result = result.Remove(index, end - index + 1);
            index = result1.IndexOf('\'');
            end = result1.IndexOf('\'', index + 1);
            result1 = result1.Remove(index, end - index + 1);
            Assert.AreEqual(expected, result);
            Assert.AreEqual(expected1, result1);


        }
    }
}
