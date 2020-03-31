using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlightSimulatorApp.Controls {
    /// <summary>
    /// Interaction logic for ConnectionControl.xaml
    /// </summary>
    public partial class ConnectionControl : UserControl {
        public delegate void ConnectEvent(string address, int port);

        public event ConnectEvent onConnectEvent;
        public ConnectionControl() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (onConnectEvent != null) {
                try {
                    string ip = this.AddressTextBox.Text;
                    int port = Int32.Parse(this.AddressTextBox.Text);
                    onConnectEvent(ip, port);
                } catch (Exception exception) {
                    
                }
            }
        }
    }
}
