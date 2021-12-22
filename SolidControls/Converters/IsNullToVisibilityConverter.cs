using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SolidControls
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class IsNullToVisibilityConverter : IValueConverter
    {
        private static readonly Lazy<IsNullToVisibilityConverter> _instance = new Lazy<IsNullToVisibilityConverter>();
        public static IValueConverter Instance { get { return _instance.Value; } }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mode = (IsNullToVisibilityConvertMode)parameter;
            switch (mode)
            {
                case IsNullToVisibilityConvertMode.NullToCollapsed:
                    return value == null ? Visibility.Collapsed : Visibility.Visible;
                case IsNullToVisibilityConvertMode.NullToHidden:
                    return value == null ? Visibility.Hidden : Visibility.Visible;
                case IsNullToVisibilityConvertMode.NotNullToCollapsed:
                    return value != null ? Visibility.Collapsed : Visibility.Visible;
                case IsNullToVisibilityConvertMode.NotNullToHidden:
                    return value != null ? Visibility.Hidden : Visibility.Visible;
                default:
                    return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum IsNullToVisibilityConvertMode
    {
        NullToCollapsed, NullToHidden, NotNullToCollapsed, NotNullToHidden,
    }
}
