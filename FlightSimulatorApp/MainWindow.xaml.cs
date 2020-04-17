using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace FlightSimulatorApp {
    using System.Configuration;
    using System.Threading;
    using System.Threading.Tasks;
    using FlightSimulatorApp.Model;
    using FlightSimulatorApp.ViewModel;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private JoystickViewModel joystickVm;
        private DashboardViewModel dashboardVm;
        private MapViewModel mapVm;
        private ConnectionControlViewModel connectionControlVm;

        [Obsolete]
        public MainWindow() {
            InitializeComponent();
            this.ConnectionControl.AddressTextBox.Text = ConfigurationSettings.AppSettings["ip"].ToString();
            this.ConnectionControl.PortTextBox.Text = ConfigurationSettings.AppSettings["port"].ToString();
            this.initializeDummyServerViewModels();
            this.bindData();
        }
        
        private void initializeFlightGearViewModels() {
            IFlightSimulatorModel model = (Application.Current as App)?.SimulatorModel;
            this.initializeViewModels(model);
            

        }

        private void initializeDummyServerViewModels() {
            IFlightSimulatorModel model = (Application.Current as App)?.DummyServerModel;
            this.initializeViewModels(model);
        }

        private void initializeViewModels(IFlightSimulatorModel model) {
            this.joystickVm = new JoystickViewModel();
            this.dashboardVm = new DashboardViewModel();
            this.mapVm = new MapViewModel();
            this.connectionControlVm = new ConnectionControlViewModel();
            this.joystickVm.SetModel(model);
            this.mapVm.SetModel(model);
            this.dashboardVm.SetModel(model);
            this.connectionControlVm.SetModel(model);
            this.BingMap.DataContext = this.mapVm;
            this.Joystick.DataContext = this.joystickVm;
            this.ControlsDisplay.DataContext = this.dashboardVm;
        }

        private void bindData() {
            Binding myBinder = new Binding("VM_ConnectionStatus");
            myBinder.Source = this.connectionControlVm;
            myBinder.Mode = BindingMode.TwoWay;
            myBinder.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(
                this.ConnectionControl,
                Controls.ConnectionControl.ConnectionStatusProperty,
                myBinder);
            Binding ipBinder = new Binding("VM_IpAddress");
            ipBinder.Source = this.connectionControlVm;
            ipBinder.Mode = BindingMode.OneWayToSource;
            ipBinder.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(
                this.ConnectionControl,
                Controls.ConnectionControl.IpAddressProperty,
                ipBinder);
            Binding portBinder = new Binding("VM_Port");
            portBinder.Source = this.connectionControlVm;
            portBinder.Mode = BindingMode.OneWayToSource;
            portBinder.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(this.ConnectionControl, Controls.ConnectionControl.PortProperty, portBinder);
        }

        

        /// <summary>Handles the LostKeyboardFocus event of the Window control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyboardFocusChangedEventArgs"/> instance containing the event data.</param>
        private void Window_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            this.Joystick.InstanceLostFocus();
        }

        private void updateJoystickValues(double x, double y) {
            this.joystickVm.VM_Aileron = x;
            this.joystickVm.VM_Elevator = y;
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e) {
            this.Joystick.keyboardPressed(sender, e);
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.joystickVm.Stop();
            MessageBox.Show(this, "Bye bye user", "FlightSimulatorApp", MessageBoxButton.OK, MessageBoxImage.None);
            Thread.Sleep(500);
        }
    }
}