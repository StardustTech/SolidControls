using System;
using System.Globalization;
using System.Windows.Data;

namespace Stardust.OpenSource.SolidControls.Wpf
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class BooleanReverseConverter : IValueConverter
    {
        private static readonly Lazy<BooleanReverseConverter> _instance = new Lazy<BooleanReverseConverter>();
        public static IValueConverter Instance { get { return _instance.Value; } }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return ConvertInternal(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return ConvertInternal(value);
        }

        private object ConvertInternal(object value) {
            return value == null || !Boolean.TryParse(value.ToString(), out bool boolValue) ? throw new InvalidCastException() : !boolValue;
        }
    }
}
