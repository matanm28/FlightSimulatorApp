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

namespace FlightSimulatorApp {
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using FlightSimulatorApp.Model;
    using FlightSimulatorApp.ViewModel;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private FlightGearViewModel vm;
        public MainWindow() {
            InitializeComponent();
            this.vm = new FlightGearViewModel(new Model.Model());
            this.DataContext = this.vm;
            this.Joystick.CoordinatesChanged += updateJoystickValues;
            this.vm.Start("127.0.0.1", 5402);
        }

        /// <summary>Handles the LostKeyboardFocus event of the Window control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyboardFocusChangedEventArgs"/> instance containing the event data.</param>
        private void Window_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            this.Joystick.lostFocus();
        }

        private void updateJoystickValues(double x, double y) {
            this.vm.VM_Aileron = x;
            this.vm.VM_Elevator = y;
        }

    }
}
