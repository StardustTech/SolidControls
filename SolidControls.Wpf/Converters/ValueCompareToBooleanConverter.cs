using System;
using System.Globalization;
using System.Windows.Data;

namespace Stardust.OpenSource.SolidControls.Wpf
{
    public sealed class ValueCompareToBooleanConverter : IValueConverter
    {
        private static readonly Lazy<ValueCompareToBooleanConverter> _instance = new Lazy<ValueCompareToBooleanConverter>();
        public static IValueConverter Instance { get { return _instance.Value; } }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value == parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return (bool)value ? parameter : null;
        }
    }
}
