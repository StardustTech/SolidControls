using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace SolidControls
{
    /// <summary>
    /// Interaction logic for LineChart.xaml
    /// </summary>
    public partial class LineChart : UserControl
    {
        public LineChart()
        {
            InitializeComponent();
        }

        public UIElementCollection XSliders { get { return chartKernel.XSliders; } }
        public UIElementCollection YSliders { get { return chartKernel.YSliders; } }

        #region DependencyProperties

        #region Title : string

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(LineChart), new UIPropertyMetadata(String.Empty));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        #endregion

        #region HorizontalAxisTitle : string

        public static readonly DependencyProperty HorizontalAxisTitleProperty =
            DependencyProperty.Register(nameof(HorizontalAxisTitle), typeof(string), typeof(LineChart), new UIPropertyMetadata(String.Empty));

        public string HorizontalAxisTitle
        {
            get { return (string)GetValue(HorizontalAxisTitleProperty); }
            set { SetValue(HorizontalAxisTitleProperty, value); }
        }

        #endregion

        #region VerticalAxisTitle : string

        public static readonly DependencyProperty VerticalAxisTitleProperty =
            DependencyProperty.Register(nameof(VerticalAxisTitle), typeof(string), typeof(LineChart), new UIPropertyMetadata(String.Empty));

        public string VerticalAxisTitle
        {
            get { return (string)GetValue(VerticalAxisTitleProperty); }
            set { SetValue(VerticalAxisTitleProperty, value); }
        }

        #endregion

        #region HorizontalAxisTicksAmount : int

        public static readonly DependencyProperty HorizontalAxisTicksAmountProperty =
            DependencyProperty.Register(nameof(HorizontalAxisTicksAmount), typeof(int), typeof(LineChart), new UIPropertyMetadata(10));

        public int HorizontalAxisTicksAmount
        {
            get { return (int)GetValue(HorizontalAxisTicksAmountProperty); }
            set { SetValue(HorizontalAxisTicksAmountProperty, value); }
        }

        #endregion

        #region VerticalAxisTicksAmount : int

        public static readonly DependencyProperty VerticalAxisTicksAmountProperty =
            DependencyProperty.Register(nameof(VerticalAxisTicksAmount), typeof(int), typeof(LineChart), new UIPropertyMetadata(10));

        public int VerticalAxisTicksAmount
        {
            get { return (int)GetValue(VerticalAxisTicksAmountProperty); }
            set { SetValue(VerticalAxisTicksAmountProperty, value); }
        }

        #endregion

        #region HorizontalAxisTickStringFormat : string

        public static readonly DependencyProperty HorizontalAxisTickStringFormatProperty =
            DependencyProperty.Register(nameof(HorizontalAxisTickStringFormat), typeof(string), typeof(LineChart), new PropertyMetadata("{0}"));

        public string HorizontalAxisTickStringFormat
        {
            get { return (string)GetValue(HorizontalAxisTickStringFormatProperty); }
            set { SetValue(HorizontalAxisTickStringFormatProperty, value); }
        }

        #endregion

        #region VerticalAxisTickStringFormat : string

        public static readonly DependencyProperty VerticalAxisTickStringFormatProperty =
            DependencyProperty.Register(nameof(VerticalAxisTickStringFormat), typeof(string), typeof(LineChart), new PropertyMetadata("{0}"));

        public string VerticalAxisTickStringFormat
        {
            get { return (string)GetValue(VerticalAxisTickStringFormatProperty); }
            set { SetValue(VerticalAxisTickStringFormatProperty, value); }
        }

        #endregion

        #region AutoResetHorizontalAxis : bool

        public bool AutoResetHorizontalAxis
        {
            get { return (bool)GetValue(AutoResetHorizontalAxisProperty); }
            set { SetValue(AutoResetHorizontalAxisProperty, value); }
        }

        public static readonly DependencyProperty AutoResetHorizontalAxisProperty =
            DependencyProperty.Register(nameof(AutoResetHorizontalAxis), typeof(bool), typeof(LineChart),
                new PropertyMetadata(true, (d, e) => (d as LineChart).chartKernel.AutoResetZoomX = (bool)e.NewValue));

        #endregion

        #region AutoResetVerticalAxis : bool

        public bool AutoResetVerticalAxis
        {
            get { return (bool)GetValue(AutoResetVerticalAxisProperty); }
            set { SetValue(AutoResetVerticalAxisProperty, value); }
        }

        public static readonly DependencyProperty AutoResetVerticalAxisProperty =
            DependencyProperty.Register(nameof(AutoResetVerticalAxis), typeof(bool), typeof(LineChart),
                new PropertyMetadata(true, (d, e) => (d as LineChart).chartKernel.AutoResetZoomY = (bool)e.NewValue));

        #endregion

        #region DataBorders

        //#region MinimumX : double

        //public static readonly DependencyProperty MinimumXProperty =
        //    LineChartKernel.MinXProperty.AddOwner(typeof(LineChart), new FrameworkPropertyMetadata(0.0,
        //        (d, e) => (d as LineChart).OnMinimumXPropertyChanged((double)e.OldValue, (double)e.NewValue),
        //        (d, v) => (d as LineChart).MinimumXPropertyValueCoerce((double)v))
        //    {
        //        BindsTwoWayByDefault = true
        //    });

        //protected virtual void OnMinimumXPropertyChanged(double oldValue, double newValue) { }

        //protected virtual double MinimumXPropertyValueCoerce(double newValue)
        //{
        //    if (newValue > MaximumX)
        //    {
        //        MaximumX = newValue;
        //    }

        //    return newValue;
        //}

        //public double MinimumX
        //{
        //    get { return (double)GetValue(MinimumXProperty); }
        //    set { SetValue(MinimumXProperty, value); }
        //}

        //#endregion

        //#region MaximumX : double

        //public static readonly DependencyProperty MaximumXProperty =
        //    LineChartKernel.MaxXProperty.AddOwner(typeof(LineChart), new PropertyMetadata(100.0,
        //        (d, e) => (d as LineChart).OnMaximumXPropertyChanged((double)e.OldValue, (double)e.NewValue),
        //        (d, v) => (d as LineChart).MaximumXPropertyValueCoerce((double)v)));

        //protected virtual void OnMaximumXPropertyChanged(double oldValue, double newValue) { }

        //protected virtual double MaximumXPropertyValueCoerce(double newValue)
        //{
        //    if (newValue < MinimumX)
        //    {
        //        MinimumX = newValue;
        //    }

        //    return newValue;
        //}

        //public double MaximumX
        //{
        //    get { return (double)GetValue(MaximumXProperty); }
        //    set { SetValue(MaximumXProperty, value); }
        //}

        //#endregion

        //#region MinimumY : double

        //public static readonly DependencyProperty MinimumYProperty =
        //    LineChartKernel.MinYProperty.AddOwner(typeof(LineChart), new FrameworkPropertyMetadata(0.0,
        //        (d, e) => (d as LineChart).OnMinimumYPropertyChanged((double)e.OldValue, (double)e.NewValue),
        //        (d, v) => (d as LineChart).MinimumYPropertyValueCoerce((double)v)));

        //protected virtual void OnMinimumYPropertyChanged(double oldValue, double newValue) { }

        //protected virtual double MinimumYPropertyValueCoerce(double newValue)
        //{
        //    if (newValue > MaximumY)
        //    {
        //        MaximumY = newValue;
        //    }

        //    return newValue;
        //}

        //public double MinimumY
        //{
        //    get { return (double)GetValue(MinimumYProperty); }
        //    set { SetValue(MinimumYProperty, value); }
        //}

        //#endregion

        //#region MaximumY : double

        //public static readonly DependencyProperty MaximumYProperty =
        //    LineChartKernel.MaxYProperty.AddOwner(typeof(LineChart), new PropertyMetadata(100.0,
        //        (d, e) => (d as LineChart).OnMaximumYPropertyChanged((double)e.OldValue, (double)e.NewValue),
        //        (d, v) => (d as LineChart).MaximumYPropertyValueCoerce((double)v)));

        //protected virtual void OnMaximumYPropertyChanged(double oldValue, double newValue) { }

        //protected virtual double MaximumYPropertyValueCoerce(double newValue)
        //{
        //    if (newValue < MinimumY)
        //    {
        //        MinimumY = newValue;
        //    }

        //    return newValue;
        //}

        //public double MaximumY
        //{
        //    get { return (double)GetValue(MaximumYProperty); }
        //    set { SetValue(MaximumYProperty, value); }
        //}

        //#endregion

        #region DefaultMinimumY : double

        public double DefaultMinimumY
        {
            get { return (double)GetValue(DefaultMinimumYProperty); }
            set { SetValue(DefaultMinimumYProperty, value); }
        }

        public static readonly DependencyProperty DefaultMinimumYProperty =
            DependencyProperty.Register(nameof(DefaultMinimumY), typeof(double), typeof(LineChart),
                new PropertyMetadata(Double.NaN,
                    (d, e) => (d as LineChart).OnDefaultMinimumYPropertyChanged((double)e.OldValue, (double)e.NewValue),
                    (d, v) => (d as LineChart).DefaultMinimumYPropertyValueCoerce((double)v)));

        protected virtual void OnDefaultMinimumYPropertyChanged(double oldValue, double newValue)
        {
            chartKernel.SetDefaultYBorders(newValue, DefaultMaximumY);
        }

        protected virtual double DefaultMinimumYPropertyValueCoerce(double newValue)
        {
            return Double.IsNaN(DefaultMaximumY) || newValue < DefaultMaximumY
                ? newValue
                : throw new ArgumentOutOfRangeException(nameof(DefaultMinimumY));
        }

        #endregion

        #region DefaultMaximumY : double

        public double DefaultMaximumY
        {
            get { return (double)GetValue(DefaultMaximumYProperty); }
            set { SetValue(DefaultMaximumYProperty, value); }
        }

        public static readonly DependencyProperty DefaultMaximumYProperty =
            DependencyProperty.Register(nameof(DefaultMaximumY), typeof(double), typeof(LineChart),
                new PropertyMetadata(Double.NaN,
                    (d, e) => (d as LineChart).OnDefaultMaximumYPropertyChanged((double)e.OldValue, (double)e.NewValue),
                    (d, v) => (d as LineChart).DefaultMaximumYPropertyValueCoerce((double)v)));

        protected virtual void OnDefaultMaximumYPropertyChanged(double oldValue, double newValue)
        {
            chartKernel.SetDefaultYBorders(DefaultMinimumY, newValue);
        }

        protected virtual double DefaultMaximumYPropertyValueCoerce(double newValue)
        {
            return Double.IsNaN(DefaultMinimumY) || newValue > DefaultMinimumY
                ? newValue
                : throw new ArgumentOutOfRangeException(nameof(DefaultMaximumY));
        }

        #endregion

        #endregion

        #region Points : PointCollection

        public static readonly DependencyProperty PointsProperty =
            LineChartKernel.PointsProperty.AddOwner(typeof(LineChart));

        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        #endregion

        #region StrokeColor : Color

        public static readonly DependencyProperty StrokeColorProperty =
            DependencyProperty.Register(nameof(StrokeColor), typeof(Color), typeof(LineChart), new PropertyMetadata(Colors.Green));

        public Color StrokeColor
        {
            get { return (Color)GetValue(StrokeColorProperty); }
            set { SetValue(StrokeColorProperty, value); }
        }

        #endregion

        #endregion
    }
}
