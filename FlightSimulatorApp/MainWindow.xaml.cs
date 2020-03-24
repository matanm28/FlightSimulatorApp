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

namespace FlightSimulatorApp
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Class1.test();
        }


        /// <summary>Handles the LostKeyboardFocus event of the Window control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyboardFocusChangedEventArgs"/> instance containing the event data.</param>
        private void Window_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.Joystick.lostFocus();
        }

        private void Throttle_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Slider slider = sender as Slider;
            slider.Value = 0;
        }
    }
}
