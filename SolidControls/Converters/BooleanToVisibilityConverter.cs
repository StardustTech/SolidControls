using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace SolidControls
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        private static readonly Lazy<BooleanToVisibilityConverter> _instance = new Lazy<BooleanToVisibilityConverter>();
        public static IValueConverter Instance { get { return _instance.Value; } }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return BooleanToVisibilityConverterHelper.Convert(value, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return BooleanToVisibilityConverterHelper.ConvertBack(value, parameter);
        }
    }

    public class MultiBooleanToVisibilityConverter : IMultiValueConverter
    {
        private static readonly Lazy<MultiBooleanToVisibilityConverter> _instance = new Lazy<MultiBooleanToVisibilityConverter>();
        public static IMultiValueConverter Instance { get { return _instance.Value; } }

        public MultiBooleanAggregationMode AggregationMode { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool[] booleanValues = values.Cast<bool>().ToArray();
            bool value = booleanValues.Aggregate((v1, v2) =>
            {
                switch (AggregationMode)
                {
                    case MultiBooleanAggregationMode.LogicalAnd:
                        return v1 && v2;
                    case MultiBooleanAggregationMode.LogicalOr:
                        return v1 || v2;
                    case MultiBooleanAggregationMode.LogicalXor:
                        return v1 ^ v2;
                    default:
                        throw new InvalidEnumArgumentException();
                }
            });

            return BooleanToVisibilityConverterHelper.Convert(value, parameter);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum BooleanToVisibilityConvertMode
    {
        FalseToCollapsed,
        FalseToHidden,
        TrueToCollapsed,
        TrueToHidden,
    }

    public enum MultiBooleanAggregationMode
    {
        LogicalAnd, LogicalOr, LogicalXor,
    }

    internal static class BooleanToVisibilityConverterHelper
    {
        public static Visibility Convert(object value, object parameter)
        {
            if (!(value is bool))
                throw new InvalidCastException();

            var mode = BooleanToVisibilityConvertMode.FalseToCollapsed;

            if (parameter is BooleanToVisibilityConvertMode)
                mode = (BooleanToVisibilityConvertMode)parameter;

            switch (mode)
            {
                case BooleanToVisibilityConvertMode.FalseToCollapsed:
                    return (bool)value ? Visibility.Visible : Visibility.Collapsed;
                case BooleanToVisibilityConvertMode.FalseToHidden:
                    return (bool)value ? Visibility.Visible : Visibility.Hidden;
                case BooleanToVisibilityConvertMode.TrueToCollapsed:
                    return (bool)value ? Visibility.Collapsed : Visibility.Visible;
                case BooleanToVisibilityConvertMode.TrueToHidden:
                    return (bool)value ? Visibility.Hidden : Visibility.Visible;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        public static bool ConvertBack(object value, object parameter)
        {
            if (!(value is Visibility))
                throw new InvalidCastException();

            var mode = BooleanToVisibilityConvertMode.FalseToCollapsed;

            if (parameter is BooleanToVisibilityConvertMode)
                mode = (BooleanToVisibilityConvertMode)parameter;

            switch (mode)
            {
                case BooleanToVisibilityConvertMode.FalseToCollapsed:
                    return (Visibility)value != Visibility.Collapsed;
                case BooleanToVisibilityConvertMode.FalseToHidden:
                    return (Visibility)value != Visibility.Hidden;
                case BooleanToVisibilityConvertMode.TrueToCollapsed:
                    return (Visibility)value == Visibility.Collapsed;
                case BooleanToVisibilityConvertMode.TrueToHidden:
                    return (Visibility)value == Visibility.Hidden;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}
