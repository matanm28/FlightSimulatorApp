using System.Windows;

namespace FlightSimulatorApp {
    using FlightSimulatorApp.Model;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        /// <summary>
        /// Gets the simulator model.
        /// </summary>
        /// <value>
        /// The simulator model.
        /// </value>
        public IFlightSimulatorModel SimulatorModel { get; private set; }
        /// <summary>
        /// Gets the dummy server model.
        /// </summary>
        /// <value>
        /// The dummy server model.
        /// </value>
        public IFlightSimulatorModel DummyServerModel { get; private set; }

        /// <summary>
        /// Handles the OnStartup event of the App control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="StartupEventArgs"/> instance containing the event data.</param>
        private void App_OnStartup(object sender, StartupEventArgs e) {
            this.SimulatorModel = new FlightSimulatorModel(new FlightGearTCPHandler());
            this.DummyServerModel = new FlightSimulatorModel(new DummyServerTCPHandler());
        }
    }
}
