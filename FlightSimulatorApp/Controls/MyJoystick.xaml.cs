using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlightSimulatorApp.Controls {
    /// <summary>
    /// Interaction logic for MyJoystick.xaml
    /// </summary>
    public partial class MyJoystick : UserControl {
        public MyJoystick() {
            InitializeComponent();
        }

        public void keyboardPressed(object sender, KeyEventArgs e) {
            if (e.Key == Key.Up) {
                this.Throttle.Value += this.Throttle.SmallChange;
            } else if (e.Key == Key.Down) {
                this.Throttle.Value -= this.Throttle.SmallChange;
            } else if (e.Key == Key.Right) {
                this.Rudder.Value += this.Rudder.SmallChange;
            } else if (e.Key == Key.Left) {
                this.Rudder.Value -= this.Rudder.SmallChange;
            }
        }

        private void slider_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            Slider slider = sender as Slider;
            slider.Value = 0;
        }

        public void lostFocus() {
            this.Joystick.lostFocus();
        }
    }
}
