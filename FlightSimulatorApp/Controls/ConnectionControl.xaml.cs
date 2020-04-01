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
    using System.ComponentModel;
    using System.Data;
    using System.Timers;
    using FlightSimulatorApp.ViewModel;

    /// <summary>
    /// Interaction logic for ConnectionControl.xaml
    /// </summary>
    public partial class ConnectionControl : UserControl{
        public enum Status { waitingForConnection, running, disconnected }

        public delegate void ConnectEvent(string address, int port);
        public event ConnectEvent onConnectEvent;
        
        private const string disconnected = "Disconnected";
        private const string waiting = "Waiting for connection";
        private const string connected = "Connected to Simulator";
        private string displayText = disconnected;
        private Brush colorBrush = Brushes.Gray;
        private Timer timer = new Timer();
        private bool toggleLight = true;
        public ConnectionControl() {
            InitializeComponent();
        }

        public static DependencyProperty ConnectionStatusProperty = DependencyProperty.Register("ConnectionStatus", typeof(Status), typeof(ConnectionControl), new PropertyMetadata(Status.disconnected));

        public Status ConnectionStatus {
            get { return (Status)GetValue(ConnectionStatusProperty); }
            set { SetValue(ConnectionStatusProperty, value); }
        }

        private static void OnConnectionStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ConnectionControl myConnectionControl = d as ConnectionControl;
            myConnectionControl.OnConnectionStatusChanged(e);
        }

        

        private void OnConnectionStatusChanged(DependencyPropertyChangedEventArgs e) {
            Status status = Status.disconnected;
            if (e.NewValue.GetType() == typeof(Status)) {
                status = (Status)e.NewValue;
            }
            switch (status) {
                case Status.running:
                    this.colorBrush = Brushes.LightGreen;
                    this.displayText = connected;
                    break;
                case Status.disconnected:
                    this.colorBrush = Brushes.Red;
                    this.displayText = disconnected;
                    break;
                case Status.waitingForConnection:
                    this.colorBrush = Brushes.Gray;
                    this.displayText = waiting;
                    break;
            }
            this.animation();
        }

        

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            this.statusTextBlock.Text = this.displayText;
            this.timer.Interval = 500;
            this.timer.Elapsed += timer_Tick;
        }

        private void timer_Tick(object sender, EventArgs e) {
            Dispatcher.Invoke(this.animation);
        }

        

        private void animation() {
            if (this.toggleLight) {
                this.statusTextBlock.Background = this.colorBrush;
            } else {
                this.statusTextBlock.Background = Brushes.Gray;
            }

            this.toggleLight = !this.toggleLight;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (onConnectEvent != null) {
                try {
                    string ip = this.AddressTextBox.Text;
                    int port = int.Parse(this.PortTextBox.Text);
                    onConnectEvent(ip, port);
                    this.timer.Start();
                } catch (Exception exception) {
                }
            }
        }

        private void AddressTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            try {
                if (this.AddressTextBox.Text == string.Empty) {
                    this.AddressTextBox.Background = Brushes.White;
                }

                string[] stringArr = this.AddressTextBox.Text.Split(".".ToCharArray());

                if (validateIP(stringArr)) {
                    this.AddressTextBox.Background = Brushes.LightGreen;
                } else {
                    this.AddressTextBox.Background = Brushes.Red;
                }
            } catch (Exception exception) {
                this.AddressTextBox.Background = Brushes.Red;
            }
        }

        private bool validateIP(string[] stringArr) {
            if (stringArr.Length != 4) {
                return false;
            }

            foreach (string octet in stringArr) {
                try {
                    int num = int.Parse(octet);
                    if (!(num >= 0 && num <= 255)) {
                        return false;
                    }
                } catch (Exception e) {
                    return false;
                }
            }

            return true;
        }

        private void PortTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            try {
                if (this.PortTextBox.Text.Length == 0) {
                    this.PortTextBox.Background = Brushes.White;
                }

                int port = int.Parse(this.PortTextBox.Text);
                if (port >= 1024 && port <= Math.Pow(2, 16)) {
                    this.PortTextBox.Background = Brushes.LightGreen;
                } else {
                    this.PortTextBox.Background = Brushes.Red;
                }
            } catch (Exception exception) {
                this.PortTextBox.Background = Brushes.Red;
            }
        }


    }
}