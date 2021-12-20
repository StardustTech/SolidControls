using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SolidControls {
    public sealed class ValueCollectionToPointCollectionConverter : IValueConverter {
        private static readonly Lazy<ValueCollectionToPointCollectionConverter> _instance = new Lazy<ValueCollectionToPointCollectionConverter>();
        public static IValueConverter Instance { get { return _instance.Value; } }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var enumerable = value as IEnumerable;
            if (enumerable == null)
                return new PointCollection();

            var pointCollection = new PointCollection();
            int count = 0;
            foreach (object item in enumerable) {
                pointCollection.Add(new Point(count++, System.Convert.ToDouble(item)));
            }

            return pointCollection;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
