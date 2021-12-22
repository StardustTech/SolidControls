using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SolidControls
{
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class EmptyStringToVisibilityConverter : IValueConverter
    {
        private static readonly Lazy<EmptyStringToVisibilityConverter> _instance = new Lazy<EmptyStringToVisibilityConverter>();
        public static IValueConverter Instance { get { return _instance.Value; } }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string valueString = value as string;
            if (!String.IsNullOrWhiteSpace(valueString))
                return Visibility.Visible;

            var convertMode = parameter is EmptyStringToVisibilityConvertMode ? (EmptyStringToVisibilityConvertMode)parameter : EmptyStringToVisibilityConvertMode.EmptyStringToCollapsed;
            switch (convertMode)
            {
                case EmptyStringToVisibilityConvertMode.EmptyStringToCollapsed:
                    return Visibility.Collapsed;
                case EmptyStringToVisibilityConvertMode.EmptyStringToHidden:
                    return Visibility.Hidden;
                default:
                    return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum EmptyStringToVisibilityConvertMode
    {
        EmptyStringToCollapsed,
        EmptyStringToHidden,
    }
}
