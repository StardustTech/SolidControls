using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Stardust.OpenSource.SolidControls.Wpf
{
    public enum BooleanToVisibilityConvertMode
    {
        FalseToCollapsed,
        FalseToHidden,
        TrueToCollapsed,
        TrueToHidden,
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        private static readonly Lazy<BooleanToVisibilityConverter> _instance = new Lazy<BooleanToVisibilityConverter>();
        public static IValueConverter Instance { get { return _instance.Value; } }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return BooleanToVisibilityConverterHelper.Convert(value, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return BooleanToVisibilityConverterHelper.ConvertBack(value, parameter);
        }
    }

    public class MultiBooleanToVisibilityConverter : IMultiValueConverter
    {
        private static readonly Lazy<MultiBooleanToVisibilityConverter> _instance = new Lazy<MultiBooleanToVisibilityConverter>();
        public static IMultiValueConverter Instance { get { return _instance.Value; } }

        public MultiBooleanConvertMode AggregationMode { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            bool[] booleanValues = values.Cast<bool>().ToArray();
            bool value = booleanValues.Aggregate((v1, v2) => {
                switch (AggregationMode) {
                case MultiBooleanConvertMode.And:
                    return v1 && v2;
                case MultiBooleanConvertMode.Or:
                    return v1 || v2;
                case MultiBooleanConvertMode.Xor:
                    return v1 ^ v2;
                default:
                    throw new InvalidEnumArgumentException();
                }
            });

            return BooleanToVisibilityConverterHelper.Convert(value, parameter);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    internal static class BooleanToVisibilityConverterHelper
    {
        public static Visibility Convert(object value, object parameter) {
            if (!(value is bool flag))
                throw new InvalidCastException();

            if (!(parameter is BooleanToVisibilityConvertMode mode)) {
                mode = BooleanToVisibilityConvertMode.FalseToCollapsed;
            }

            switch (mode) {
            case BooleanToVisibilityConvertMode.FalseToCollapsed:
                return flag ? Visibility.Visible : Visibility.Collapsed;
            case BooleanToVisibilityConvertMode.FalseToHidden:
                return flag ? Visibility.Visible : Visibility.Hidden;
            case BooleanToVisibilityConvertMode.TrueToCollapsed:
                return flag ? Visibility.Collapsed : Visibility.Visible;
            case BooleanToVisibilityConvertMode.TrueToHidden:
                return flag ? Visibility.Hidden : Visibility.Visible;
            default:
                throw new InvalidEnumArgumentException();
            }
        }

        public static bool ConvertBack(object value, object parameter) {
            if (!(value is Visibility visibility))
                throw new InvalidCastException();

            if (!(parameter is BooleanToVisibilityConvertMode mode)) {
                mode = BooleanToVisibilityConvertMode.FalseToCollapsed;
            }

            switch (mode) {
            case BooleanToVisibilityConvertMode.FalseToCollapsed:
                return visibility != Visibility.Collapsed;
            case BooleanToVisibilityConvertMode.FalseToHidden:
                return visibility != Visibility.Hidden;
            case BooleanToVisibilityConvertMode.TrueToCollapsed:
                return visibility == Visibility.Collapsed;
            case BooleanToVisibilityConvertMode.TrueToHidden:
                return visibility == Visibility.Hidden;
            default:
                throw new InvalidEnumArgumentException();
            }
        }
    }
}
