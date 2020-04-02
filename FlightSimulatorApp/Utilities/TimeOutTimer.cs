using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.Utilities {
    using System.Diagnostics;

    class TimeOutTimer {
        private Stopwatch timer = new Stopwatch();
        private TimeSpan time;
        private bool timePassed = false;
        public bool TimePassed {
            get {
                if (!this.timePassed) {
                    this.checkTime();
                }

                return this.timePassed;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TimeOutTimer(int seconds=10) {
            this.time = new TimeSpan(0,0,0,seconds);
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TimeOutTimer(int minutes, int seconds) {
            this.time = new TimeSpan(0, 0, minutes, seconds);
        }


        public void Start() {
            this.timer.Start();
        }

        public void Stop() {
            this.timer.Stop();
        }

        public void Reset() {
            this.timer.Reset();
            this.timePassed = false;
        }

        private void checkTime() {
            if (this.timer.Elapsed > this.time) {
                this.timePassed = true;
            }
        }
    }
}
