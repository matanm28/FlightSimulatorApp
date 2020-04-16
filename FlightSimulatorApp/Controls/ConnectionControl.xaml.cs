using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FlightSimulatorApp.Controls {
    using System.Timers;

    /// <summary>
    /// Interaction logic for ConnectionControl.xaml
    /// </summary>
    public partial class ConnectionControl : UserControl {
        public enum Status { active, connect, disconnect, inActive }

        private const float Second = 1000;
        private const string disconnected = "Simulator Disconnected";
        private const string connected = "Simulator Connected";
        private string displayText = disconnected;
        private Timer timer = new Timer();
        private bool toggleLight = true;
        private bool error = false;
        private string ipAddress;
        private int port;
        private Status connectionStatus;

        /// <summary>
        /// The connection status property
        /// </summary>
        public static readonly DependencyProperty ConnectionStatusProperty = DependencyProperty.Register(
            "ConnectionStatus",
            typeof(Status),
            typeof(ConnectionControl),
            new FrameworkPropertyMetadata(Status.inActive, OnConnectionStatusPropertyChanged));

        /// <summary>
        /// Gets or sets the connection status.
        /// </summary>
        /// <value>
        /// The connection status.
        /// </value>
        public Status ConnectionStatus {
            get { return (Status)GetValue(ConnectionStatusProperty); }
            set { SetValue(ConnectionStatusProperty, value); }
        }

        /// <summary>
        /// Called when [connection status property changed].
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnConnectionStatusPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            (obj as ConnectionControl).OnConnectionStatusPropertyChanged(e);
        }
        /// <summary>
        /// Raises the <see cref="E:ConnectionStatusPropertyChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnConnectionStatusPropertyChanged(DependencyPropertyChangedEventArgs e) {
            Status value = (Status)e.NewValue;
            this.timer.Stop();
            switch (value) {
                case Status.active:
                    this.startDisplayText(connected, true);
                    this.changeButtonsDisplay(Status.active);
                    this.AddressTextBox.IsEnabled = false;
                    this.PortTextBox.IsEnabled = false;
                    break;
                case Status.inActive:
                    this.ErrorTextBlock.Text = string.Empty;
                    this.ErrorTextBlock.Visibility = Visibility.Collapsed;
                    this.startDisplayText(disconnected, false);
                    this.changeButtonsDisplay(Status.inActive);
                    this.AddressTextBox.IsEnabled = true;
                    this.PortTextBox.IsEnabled = true;
                    break;
                case Status.disconnect:
                case Status.connect:
                    this.StatusTextBlock.Text = "Waiting to simulator response";
                    this.StatusTextBlock.Foreground = Brushes.Black;
                    this.StatusTextBlock.Visibility = Visibility.Visible;
                    break;
            }

            this.connectionStatus = value;
        }

        /// <summary>
        /// The ip address property
        /// </summary>
        public static readonly DependencyProperty IpAddressProperty = DependencyProperty.Register(
            "IpAddress",
            typeof(string),
            typeof(ConnectionControl),
            new PropertyMetadata("127.0.0.1"));

        /// <summary>
        /// Gets the ip address.
        /// </summary>
        /// <value>
        /// The ip address.
        /// </value>
        public string IpAddress {
            get { return this.ipAddress; }
            private set { SetValue(IpAddressProperty, value); }
        }
        /// <summary>
        /// The port property
        /// </summary>
        public static readonly DependencyProperty PortProperty = DependencyProperty.Register(
            "Port",
            typeof(int),
            typeof(ConnectionControl),
            new PropertyMetadata(5402));
        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int Port {
            get { return this.port; }
            private set { SetValue(PortProperty, value); }
        }

        public ConnectionControl() {
            InitializeComponent();
        }
        /// <summary>
        /// Handles the Loaded event of the UserControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            this.ErrorTextBlock.Text = this.displayText;
            this.timer.Interval = Second;
            this.timer.Elapsed += timer_Tick;
            this.startDisplayText(disconnected, false);
        }
        /// <summary>
        /// Handles the Tick event of the timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void timer_Tick(object sender, EventArgs e) {
            try {
                Dispatcher.Invoke(this.animation);
            } catch (Exception exception) {
                Console.WriteLine(e);
            }
        }
        /// <summary>
        /// runs the text animation for this instance.
        /// </summary>
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

        private void connectionActive() {
            this.startDisplayText(connected, true);
            this.changeButtonsDisplay(Status.active);
        }
        /// <summary>
        /// Handles the Click event of the ConnectButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ConnectButton_Click(object sender, RoutedEventArgs e) {
            this.timer.Stop();
            this.ErrorTextBlock.Visibility = Visibility.Collapsed;
            try {
                string ip = this.AddressTextBox.Text;
                int port = int.Parse(this.PortTextBox.Text);
                if (validatePort(port) && validateIP(ip)) {
                    this.IpAddress = ip;
                    this.Port = port;
                    this.ConnectionStatus = Status.connect;
                } else if (!validatePort(port) && !validateIP(ip)) {
                    this.startDisplayText(disconnected, false, "Invalid Port & IP!");
                } else if (!validatePort(port)) {
                    this.ipAddress = ip;
                    this.startDisplayText(disconnected, false, "Invalid Port!");
                } else {
                    this.port = port;
                    this.startDisplayText(disconnected, false, "Invalid IP!");
                }
            } catch (Exception exception) {
                this.startDisplayText(disconnected, false, exception.Message);
            }
        }
        /// <summary>
        /// Handles the Click event of the DisconnectButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DisconnectButton_Click(object sender, RoutedEventArgs e) {
            this.ConnectionStatus = Status.disconnect;
        }
        /// <summary>
        /// Changes the buttons display.
        /// </summary>
        /// <param name="connectionStatus">The connection status.</param>
        private void changeButtonsDisplay(Status connectionStatus) {
            switch (connectionStatus) {
                case Status.active:
                    this.ConnectButton.IsEnabled = false;
                    this.ConnectButton.Visibility = Visibility.Collapsed;
                    this.DisconnectButton.IsEnabled = true;
                    this.DisconnectButton.Visibility = Visibility.Visible;
                    break;
                case Status.inActive:
                    this.ConnectButton.IsEnabled = true;
                    this.ConnectButton.Visibility = Visibility.Visible;
                    this.DisconnectButton.IsEnabled = false;
                    this.DisconnectButton.Visibility = Visibility.Collapsed;
                    break;
                case Status.disconnect:
                    this.DisconnectButton.IsEnabled = false;
                    break;
                case Status.connect:
                    this.ConnectButton.IsEnabled = false;
                    break;
            }

            this.error = false;
        }
        /// <summary>
        /// Starts the display text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="connected">if set to <c>true</c> [connected].</param>
        /// <param name="errorString">The error string.</param>
        private void startDisplayText(string text, bool connected, string errorString = "") {
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
        /// <summary>
        /// Handles the TextChanged event of the AddressTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void AddressTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            if (this.AddressTextBox.Text == string.Empty) {
                this.AddressTextBox.Background = Brushes.White;
            } else if (validateIP(this.AddressTextBox.Text)) {
                this.AddressTextBox.Background = Brushes.LightGreen;
            } else {
                this.AddressTextBox.Background = Brushes.Red;
            }
        }
        /// <summary>
        /// Validates the ip.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Handles the TextChanged event of the PortTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
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
        /// <summary>
        /// Validates the port.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns></returns>
        private static bool validatePort(int port) {
            return port >= 1024 && port <= Math.Pow(2, 16);
        }
    }
}