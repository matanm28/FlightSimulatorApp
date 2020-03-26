using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.Utilities {
    class BiDictionary<TKey, TValue> : Dictionary<TKey, TValue> {
        
        private Dictionary<TValue, TKey> keyToValue;
        public TValue this[TKey key] {
            get {
                return base[key];
            }
            set {
                base[key] = value;
                this.keyToValue[value] = key;
            }
        }

        public TKey this[TValue serachValue] {
            get { return this.keyToValue[serachValue]; }
            set {
                this.keyToValue[serachValue] = value;
                base[value] = serachValue;
            }
        }

        public TKey getKeyFromValue(TValue value) {
            return this.keyToValue[value];
        }

        public void Add(TKey key, TValue value) {
            base.Add(key,value);
            this.keyToValue.Add(value,key);
        }

        public void Add(TValue value, TKey key) {
            base.Add(key,value);
            this.keyToValue.Add(value,key);
        }

        public BiDictionary() : base() {
            this.keyToValue = new Dictionary<TValue, TKey>();
        }
    }
}
