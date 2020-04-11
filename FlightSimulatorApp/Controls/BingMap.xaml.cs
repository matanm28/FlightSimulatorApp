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
using Microsoft.Maps.MapControl.WPF;

namespace FlightSimulatorApp.Controls {
    /// <summary>
    /// Interaction logic for BingMap.xaml
    /// </summary>
    public partial class BingMap : UserControl {
        /// <summary>
        /// Initializes a new instance of the <see cref="BingMap"/> class.
        /// </summary>
        public BingMap() {
            InitializeComponent();
            MyMap.ViewChangeOnFrame += new EventHandler<MapEventArgs>(MyMap_ViewChangeOnFrame);
        }

        void MyMap_ViewChangeOnFrame(object sender, MapEventArgs e) {
            //Gets the map that raised this event
            Map map = (Map)sender;
            //Gets the bounded rectangle for the current frame
            LocationRect bounds = map.BoundingRectangle;
            //Ben-Gurion airport location:
            //32.002644, 34.888781
        }
    }
}