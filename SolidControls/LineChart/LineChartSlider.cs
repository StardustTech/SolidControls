using System.Windows;
using System.Windows.Media;

namespace SolidControls
{
    public class LineChartSlider : FrameworkElement
    {
        #region DependencyProperties

        #region Value : double

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(LineChartSlider),
                new PropertyMetadata(0.0, (d, e) => (d as LineChartSlider).OnValuePropertyChanged()));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        protected virtual void OnValuePropertyChanged() { }

        #endregion

        #region Color : Color

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(LineChartSlider));

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        #endregion

        #region IsReadOnly : bool

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(LineChartSlider));

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        #endregion

        #endregion
    }

    public class LineChartRangeSlider : LineChartSlider
    {
        #region DependencyProperties

        #region EndValue : double

        public static readonly DependencyProperty EndValueProperty =
            DependencyProperty.Register("EndValue", typeof(double), typeof(LineChartRangeSlider),
                new PropertyMetadata(0.0, (d, e) => (d as LineChartRangeSlider).OnEndValuePropertyChanged()));

        public double EndValue
        {
            get { return (double)GetValue(EndValueProperty); }
            set { SetValue(EndValueProperty, value); }
        }

        protected virtual void OnEndValuePropertyChanged()
        {
            RangeLength = EndValue - Value;
        }

        #endregion

        #region RangeLength : double

        public static readonly DependencyProperty RangeLengthProperty =
            DependencyProperty.Register("RangeLength", typeof(double), typeof(LineChartRangeSlider));

        public double RangeLength
        {
            get { return (double)GetValue(RangeLengthProperty); }
            private set { SetValue(RangeLengthProperty, value); }
        }

        #endregion

        #endregion

        #region Overrides

        protected override void OnValuePropertyChanged()
        {
            RangeLength = EndValue - Value;
        }

        #endregion
    }
}
