using System.Windows;
using System.Windows.Media;

namespace Stardust.OpenSource.SolidControls.Wpf
{
    public class LineChartSlider : FrameworkElement
    {
        #region DependencyProperties

        #region Value : double

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(LineChartSlider),
                new PropertyMetadata(0.0, (d, e) => (d as LineChartSlider).OnValuePropertyChanged()));

        public double Value {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        protected virtual void OnValuePropertyChanged() {
            RaiseSliderUpdated();
        }

        #endregion

        #region Color : Color

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(nameof(Color), typeof(Color), typeof(LineChartSlider),
                new PropertyMetadata(Colors.Yellow, (d, e) => (d as LineChartSlider).OnColorPropertyChanged()));

        public Color Color {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        protected virtual void OnColorPropertyChanged() {
            RaiseSliderUpdated();
        }

        #endregion

        #region IsReadOnly : bool

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(LineChartSlider));

        public bool IsReadOnly {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        #endregion

        #endregion

        public static readonly RoutedEvent SliderUpdatedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(SliderUpdated), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LineChartSlider));

        public event RoutedEventHandler SliderUpdated {
            add { AddHandler(SliderUpdatedEvent, value); }
            remove { RemoveHandler(SliderUpdatedEvent, value); }
        }

        protected void RaiseSliderUpdated() {
            var eventArgs = new RoutedEventArgs(SliderUpdatedEvent);
            RaiseEvent(eventArgs);
        }
    }

    public class LineChartRangeSlider : LineChartSlider
    {
        #region DependencyProperties

        #region EndValue : double

        public static readonly DependencyProperty EndValueProperty =
            DependencyProperty.Register(nameof(EndValue), typeof(double), typeof(LineChartRangeSlider),
                new PropertyMetadata(0.0, (d, e) => (d as LineChartRangeSlider).OnEndValuePropertyChanged()));

        public double EndValue {
            get { return (double)GetValue(EndValueProperty); }
            set { SetValue(EndValueProperty, value); }
        }

        protected virtual void OnEndValuePropertyChanged() {
            RangeLength = EndValue - Value;
            RaiseSliderUpdated();
        }

        #endregion

        #region RangeLength : double

        public static readonly DependencyProperty RangeLengthProperty =
            DependencyProperty.Register(nameof(RangeLength), typeof(double), typeof(LineChartRangeSlider),
                new PropertyMetadata(0.0, (d, e) => (d as LineChartRangeSlider).OnRangeLengthPropertyChanged()));

        public double RangeLength {
            get { return (double)GetValue(RangeLengthProperty); }
            private set { SetValue(RangeLengthProperty, value); }
        }

        protected virtual void OnRangeLengthPropertyChanged() {
            RaiseSliderUpdated();
        }

        #endregion

        #endregion

        #region Overrides

        protected override void OnValuePropertyChanged() {
            RangeLength = EndValue - Value;
            base.OnValuePropertyChanged();
        }

        #endregion
    }
}
