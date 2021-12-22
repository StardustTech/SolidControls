﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SolidControls
{
    [ValueConversion(typeof(Enum), typeof(IEnumerable<Enum>))]
    public class EnumToValueCollectionConverter : IValueConverter
    {
        private static readonly Lazy<EnumToValueCollectionConverter> _instance = new Lazy<EnumToValueCollectionConverter>();
        public static IValueConverter Instance { get { return _instance.Value; } }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Enum))
                return new Enum[0];
            return Enum.GetValues(value.GetType()).Cast<Enum>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
