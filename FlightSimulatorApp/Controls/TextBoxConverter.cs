using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.Controls {
    using System.Globalization;
    using System.IO;
    using System.Windows.Data;

    [ValueConversion(typeof(DateTime), typeof(String))]
    class TextBoxConverter : IValueConverter {
        /// <summary>Converts a value. </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            try {
                double tempValue = (double)value;
                if (tempValue == (int)tempValue) {
                    return tempValue.ToString() + ".000";
                }
                string data = tempValue.ToString("####.#####");
                return data;
            } catch (InvalidCastException e) {
                return "Casting error";
            } catch (Exception e) {
                return "Exception error";
            }
        }

        /// <summary>Converts a value. </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            string tempValue = value as string;
            if (tempValue != null) {
                try {
                    return Double.Parse(tempValue);
                } catch (Exception e) {
                    Console.WriteLine(e);
                    return 0;
                }
            }
            return 0;
        }
    }
}
