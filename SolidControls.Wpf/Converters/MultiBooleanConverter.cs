using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Stardust.OpenSource.SolidControls.Wpf
{
    public class MultiBooleanConverter : IMultiValueConverter
    {
        private static readonly Lazy<MultiBooleanConverter> _instance = new Lazy<MultiBooleanConverter>();
        public static IMultiValueConverter Instance { get { return _instance.Value; } }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if (values == null || values.Length == 0 || values.Any(value => !(value is bool)))
                return false;

            var boolValues = values.Cast<bool>();

            var mode = (MultiBooleanConvertMode)parameter;
            switch (mode) {
            case MultiBooleanConvertMode.And:
                return boolValues.Aggregate((v1, v2) => v1 && v2);
            case MultiBooleanConvertMode.Or:
                return boolValues.Aggregate((v1, v2) => v1 || v2);
            case MultiBooleanConvertMode.Xor:
                return boolValues.Aggregate((v1, v2) => v1 ^ v2);
            default:
                return Binding.DoNothing;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public enum MultiBooleanConvertMode { And, Or, Xor, }
}
