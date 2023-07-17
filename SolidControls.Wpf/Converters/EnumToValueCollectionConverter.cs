using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Stardust.OpenSource.SolidControls.Wpf
{
    [ValueConversion(typeof(Enum), typeof(IEnumerable<Enum>))]
    public class EnumToValueCollectionConverter : IValueConverter
    {
        private static readonly Lazy<EnumToValueCollectionConverter> _instance = new Lazy<EnumToValueCollectionConverter>();
        public static IValueConverter Instance { get { return _instance.Value; } }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value is Enum ? (object)Enum.GetValues(value.GetType()).Cast<Enum>() : new Enum[0];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
