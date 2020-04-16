using System;

namespace FlightSimulatorApp.Utilities {
    using System.Diagnostics;

    /// <summary>
    /// a time out timer for general uses.
    /// </summary>
    public class TimeOutTimer {
        /// <summary>
        /// The timer
        /// </summary>
        private Stopwatch timer = new Stopwatch();
        /// <summary>
        /// The time constrain.
        /// </summary>
        private TimeSpan time;
        /// <summary>
        /// indicates if time has passed
        /// </summary>
        private bool timePassed;
        /// <summary>
        /// Gets a value indicating whether [time passed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [time passed]; otherwise, <c>false</c>.
        /// </value>
        public bool TimePassed {
            get {
                if (!this.timePassed) {
                    this.checkTime();
                }

                return this.timePassed;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TimeOutTimer(int seconds = 10) {
            this.time = new TimeSpan(0, 0, 0, seconds);
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TimeOutTimer(int minutes, int seconds) {
            this.time = new TimeSpan(0, 0, minutes, seconds);
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start() {
            this.timer.Start();
        }
        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop() {
            this.timer.Stop();
        }
        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset() {
            this.timer.Reset();
            this.timePassed = false;
        }
        /// <summary>
        /// Checks the time.
        /// </summary>
        private void checkTime() {
            if (this.timer.Elapsed > this.time) {
                this.timePassed = true;
            }
        }
    }
}
