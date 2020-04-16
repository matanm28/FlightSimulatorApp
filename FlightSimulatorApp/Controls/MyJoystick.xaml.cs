using System.Windows.Controls;
using System.Windows.Input;

namespace FlightSimulatorApp.Controls {
    /// <summary>
    /// Interaction logic for MyJoystick.xaml
    /// </summary>
    public partial class MyJoystick : UserControl {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyJoystick"/> class.
        /// </summary>
        public MyJoystick() {
            InitializeComponent();
        }
        /// <summary>
        /// Keyboards the pressed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// a method to apply if this instance lost focus
        /// </summary>
        public void InstanceLostFocus() {
            this.Joystick.InstanceLostFocus();
        }
        /// <summary>
        /// Handles the MouseDoubleClick event of the slider control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void slider_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            Slider slider = sender as Slider;
            slider.Value = 0;
        }

    }
}
