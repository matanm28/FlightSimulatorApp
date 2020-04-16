using System.Collections.Generic;

namespace FlightSimulatorApp.Utilities {
    /// <summary>
    /// Two sided dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <seealso cref="System.Collections.Generic.Dictionary{TKey, TValue}" />
    public class BiDictionary<TKey, TValue> : Dictionary<TKey, TValue> {
        /// <summary>
        /// a dictionary that holds a value-to-key mapping.
        /// </summary>
        private Dictionary<TValue, TKey> keyToValue;

        /// <summary>
        /// Gets or sets the <see cref="TValue"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="TValue"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public TValue this[TKey key] {
            get { return base[key]; }
            set {
                base[key] = value;
                this.keyToValue[value] = key;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="TKey"/> with the specified search value.
        /// </summary>
        /// <value>
        /// The <see cref="TKey"/>.
        /// </value>
        /// <param name="searchValue">The search value.</param>
        /// <returns></returns>
        public TKey this[TValue searchValue] {
            get { return this.keyToValue[searchValue]; }
            set {
                this.keyToValue[searchValue] = value;
                base[value] = searchValue;
            }
        }

        /// <summary>
        /// Gets the key from value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public TKey getKeyFromValue(TValue value) {
            return this.keyToValue[value];
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be <see langword="null" /> for reference types.</param>
        public void Add(TKey key, TValue value) {
            base.Add(key, value);
            this.keyToValue.Add(value, key);
        }

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="key">The key.</param>
        public void Add(TValue value, TKey key) {
            base.Add(key, value);
            this.keyToValue.Add(value, key);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BiDictionary{TKey, TValue}"/> class.
        /// </summary>
        public BiDictionary()
            : base() {
            this.keyToValue = new Dictionary<TValue, TKey>();
        }
    }
}