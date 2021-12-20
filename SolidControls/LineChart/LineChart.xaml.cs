using System;
using System.Windows;
using System.Windows.Controls;
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

        private void LineChart_Loaded(object sender, RoutedEventArgs e) { }

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

        #region Points : PointCollection

        public static readonly DependencyProperty PointsProperty =
            LineChartKernel.PointsProperty.AddOwner(typeof(LineChart));

        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        #endregion

        #region AutoResetHorizontalAxis : bool

        public static readonly DependencyProperty AutoResetHorizontalAxisProperty =
            LineChartKernel.AutoResetZoomXProperty.AddOwner(typeof(LineChart));

        public bool AutoResetHorizontalAxis
        {
            get { return (bool)GetValue(AutoResetHorizontalAxisProperty); }
            set { SetValue(AutoResetHorizontalAxisProperty, value); }
        }

        #endregion

        #region AutoResetVerticalAxis : bool

        public static readonly DependencyProperty AutoResetVerticalAxisProperty =
            LineChartKernel.AutoResetZoomYProperty.AddOwner(typeof(LineChart));

        public bool AutoResetVerticalAxis
        {
            get { return (bool)GetValue(AutoResetVerticalAxisProperty); }
            set { SetValue(AutoResetVerticalAxisProperty, value); }
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
