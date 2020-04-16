using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace FlightSimulatorApp {
    using FlightSimulatorApp.Model;
    using FlightSimulatorApp.ViewModel;
    using System.Configuration;


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private JoystickViewModel joystickVM;
        private DashboardViewModel dashboardVM;
        private MapViewModel mapVM;
        private ConnectionControlViewModel connectionControlVM;
        private bool connected = false;

        [Obsolete]
        public MainWindow() {
            InitializeComponent();
            this.ConnectionControl.AddressTextBox.Text = ConfigurationSettings.AppSettings["ip"].ToString();
            this.ConnectionControl.PortTextBox.Text = ConfigurationSettings.AppSettings["port"].ToString();
            this.initializeNewViewModel(new FlightSimulatorModel(new DummyServerTCPHandler()));
            this.bindData();
        }

        private void initializeNewViewModel() {
            IFlightSimulatorModel model = new FlightSimulatorModel();
            this.joystickVM = new JoystickViewModel(model);
            this.dashboardVM = new DashboardViewModel(model);
            this.mapVM = new MapViewModel(model);
            this.connectionControlVM = new ConnectionControlViewModel(model);
            this.BingMap.DataContext = this.mapVM;
            this.Joystick.DataContext = this.joystickVM;
            this.ControlsDisplay.DataContext = this.dashboardVM;
        }

        private void bindData() {
            Binding myBinder = new Binding("VM_ConnectionStatus");
            myBinder.Source = this.connectionControlVM;
            myBinder.Mode = BindingMode.TwoWay;
            myBinder.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(
                this.ConnectionControl,
                Controls.ConnectionControl.ConnectionStatusProperty,
                myBinder);
            Binding ipBinder = new Binding("VM_IpAddress");
            ipBinder.Source = this.connectionControlVM;
            ipBinder.Mode = BindingMode.OneWayToSource;
            ipBinder.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(
                this.ConnectionControl,
                Controls.ConnectionControl.IpAddressProperty,
                ipBinder);
            Binding portBinder = new Binding("VM_Port");
            portBinder.Source = this.connectionControlVM;
            portBinder.Mode = BindingMode.OneWayToSource;
            portBinder.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(
                this.ConnectionControl,
                Controls.ConnectionControl.PortProperty,
                portBinder);
        }

        private void initializeNewViewModel(IFlightSimulatorModel model) {
            this.joystickVM = new JoystickViewModel(model);
            this.dashboardVM = new DashboardViewModel(model);
            this.mapVM = new MapViewModel(model);
            this.connectionControlVM = new ConnectionControlViewModel(model);
            this.BingMap.DataContext = this.mapVM;
            this.Joystick.DataContext = this.joystickVM;
            this.ControlsDisplay.DataContext = this.dashboardVM;
        }


        /// <summary>Handles the LostKeyboardFocus event of the Window control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyboardFocusChangedEventArgs"/> instance containing the event data.</param>
        private void Window_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            this.Joystick.InstanceLostFocus();
        }

        private void updateJoystickValues(double x, double y) {
            this.joystickVM.VM_Aileron = x;
            this.joystickVM.VM_Elevator = y;
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e) {
            this.Joystick.keyboardPressed(sender, e);
        }

        private void Window_Closed(object sender, EventArgs e) {
            this.joystickVM.Stop();
        }
    }
}