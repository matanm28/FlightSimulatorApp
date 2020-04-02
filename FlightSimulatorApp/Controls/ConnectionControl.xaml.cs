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
    public partial class ConnectionControl : UserControl {
        public enum Status { connected, disconnected }

        public delegate void ConnectEvent(string address, int port);

        public event ConnectEvent onConnectEvent;

        public delegate void DisconnectEvent();

        public event DisconnectEvent onDisconnectEvent;

        private const float Second = 1000;
        private const string disconnected = "Disconnected";
        private const string waiting = "Waiting for connection";
        private const string connected = "Connected to Simulator";
        private string displayText = disconnected;
        private string errorText = string.Empty;
        private Brush colorBrush = Brushes.Gray;
        private Timer timer = new Timer();
        private bool toggleLight = true;
        private bool error = false;

        public ConnectionControl() {
            InitializeComponent();
        }
        
        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            this.ErrorTextBlock.Text = this.displayText;
            this.timer.Interval = Second;
            this.timer.Elapsed += timer_Tick;
            this.startDisplayText(disconnected, false);

        }

        private void timer_Tick(object sender, EventArgs e) {
            Dispatcher.Invoke(this.animation);
        }

        private void animation() {
            if (this.toggleLight) {
                this.StatusTextBlock.Visibility = Visibility.Visible;
            } else {
                this.StatusTextBlock.Visibility = Visibility.Hidden;
            }

            if (this.error) {
                if (this.toggleLight) {
                    this.ErrorTextBlock.Visibility = Visibility.Visible;
                } else {
                    this.ErrorTextBlock.Visibility = Visibility.Hidden;
                }
            }
            
            this.toggleLight = !this.toggleLight;
        }


        private void ConnectButton_Click(object sender, RoutedEventArgs e) {
            this.timer.Stop();
            if (onConnectEvent != null) {
                try {
                    string ip = this.AddressTextBox.Text;
                    int port = int.Parse(this.PortTextBox.Text);
                    if (validatePort(port) && validateIP(ip)) {
                        onConnectEvent(ip, port);
                        this.startDisplayText(connected, true);
                        this.changeButtonsDisplay(Status.connected);
                    } else if (!validatePort(port)) {
                        this.startDisplayText(disconnected, false, "Invalid Port!");
                    } else {
                        this.startDisplayText(disconnected, false, "Invalid IP");
                    }
                } catch (Exception exception) {
                    this.startDisplayText(disconnected, false, exception.Message);
                }
            }
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e) {
            if (onDisconnectEvent != null) {
                this.timer.Stop();
                onDisconnectEvent();
                this.ErrorTextBlock.Text = string.Empty;
                this.startDisplayText(disconnected,false);
                this.changeButtonsDisplay(Status.disconnected);
                
            }

        }
        
        private void changeButtonsDisplay(Status connectionStatus) {
            switch (connectionStatus) {
                case Status.connected:
                    this.ConnectButton.IsEnabled = false;
                    this.ConnectButton.Visibility = Visibility.Collapsed;
                    this.DisconnectButton.IsEnabled = true;
                    this.DisconnectButton.Visibility = Visibility.Visible;
                    break;
                case Status.disconnected:
                    this.ConnectButton.IsEnabled = true;
                    this.ConnectButton.Visibility = Visibility.Visible;
                    this.DisconnectButton.IsEnabled = false;
                    this.DisconnectButton.Visibility = Visibility.Collapsed;
                    break;
            }

            this.error = false;
        }

        private void startDisplayText(string text, bool connected, string errorString="") {
            this.StatusTextBlock.Text = "Status: " + text;
            if (connected) {
                this.StatusTextBlock.Foreground = Brushes.LightGreen;
            } else {
                this.StatusTextBlock.Foreground = Brushes.Red;
            }
            if (errorString != string.Empty) {
                this.error = true;
                this.ErrorTextBlock.Text = errorString;
            }
            this.timer.Start();
        }

        private void AddressTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            if (this.AddressTextBox.Text == string.Empty) {
                this.AddressTextBox.Background = Brushes.White;
            } else if (validateIP(this.AddressTextBox.Text)) {
                this.AddressTextBox.Background = Brushes.LightGreen;
            } else {
                this.AddressTextBox.Background = Brushes.Red;
            }
        }

        private static bool validateIP(string ip) {
            if (ip.ToUpper() == "localhost".ToUpper()) {
                return true;
            }

            string[] stringArr = ip.Split(".".ToCharArray());
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

                if (validatePort(int.Parse(this.PortTextBox.Text))) {
                    this.PortTextBox.Background = Brushes.LightGreen;
                } else {
                    this.PortTextBox.Background = Brushes.Red;
                }
            } catch (Exception exception) {
                this.PortTextBox.Background = Brushes.Red;
            }
        }

        private static bool validatePort(int port) {
            return port >= 1024 && port <= Math.Pow(2, 16);
        }

    }
}