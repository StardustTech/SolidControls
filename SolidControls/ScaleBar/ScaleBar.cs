using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace SolidControls
{
    public sealed class ScaleBar : TickBar
    {
        private bool _isTicksAmountChanged;
        private bool _isTicksAmountSet;

        public ScaleBar()
        {
            var dpDescriptor = DependencyPropertyDescriptor.FromProperty(TickFrequencyProperty, typeof(ScaleBar));
            dpDescriptor.AddValueChanged(this, (s, e) =>
            {
                if (TickFrequency > 0)
                {
                    if (!_isTicksAmountChanged)
                    {
                        _isTicksAmountSet = false;
                        InvalidateVisual();
                    }

                    _isTicksAmountChanged = false;
                }
            });
        }

        protected override void OnRender(DrawingContext context)
        {
            if (!TickStringVisible)
            {
                base.OnRender(context);
                return;
            }

            var culture = Thread.CurrentThread.CurrentUICulture;
            var fontFamily = TextElement.GetFontFamily(this);
            var style = TextElement.GetFontStyle(this);
            var weight = TextElement.GetFontWeight(this);
            var stretch = TextElement.GetFontStretch(this);
            var typeface = new Typeface(fontFamily, style, weight, stretch);
            double fontSize = TextElement.GetFontSize(this);

            // 刻度文字到刻度线的留白宽度
            const double tickTextSpacing = 2;

            var baseLinePen = new Pen(Fill, 1);
            baseLinePen.Freeze();

            // 不同布局下刻度值文字起始位置计算方法不同，通过这两个 Func 来进行处理
            Func<double /* tickPosition */, double /* textWidth|Height */, double /* textPositionX|Y */> getXFunc, getYFunc;

            // 当刻度线处于特定像素位置时显示会模糊化，使用 GuidelineSet 可避免刻度线模糊
            GuidelineSet guidelineSet;

            // 刻度线起止点
            Point lineStartPoint, lineEndPoint;

            switch (Placement)
            {
                case TickBarPlacement.Left:
                    getXFunc = (tickPosition, textWidth) => -textWidth - tickTextSpacing;
                    getYFunc = (tickPosition, textHeight) => (IsDirectionReversed ? tickPosition - Minimum : Maximum - tickPosition) / (Maximum - Minimum) * ActualHeight - textHeight / 2;
                    guidelineSet = new GuidelineSet(new[] { ActualWidth - baseLinePen.Thickness / 2, ActualWidth + baseLinePen.Thickness / 2 }, null);
                    lineStartPoint = new Point(ActualWidth, 0);
                    lineEndPoint = new Point(ActualWidth, ActualHeight);
                    break;
                case TickBarPlacement.Top:
                    getXFunc = (tickPosition, textWidth) => (IsDirectionReversed ? Maximum - tickPosition : tickPosition - Minimum) / (Maximum - Minimum) * ActualWidth - textWidth / 2;
                    getYFunc = (tickPosition, textHeight) => -textHeight - tickTextSpacing;
                    guidelineSet = new GuidelineSet(null, new[] { ActualHeight - baseLinePen.Thickness / 2, ActualHeight + baseLinePen.Thickness / 2 });
                    lineStartPoint = new Point(0, ActualHeight);
                    lineEndPoint = new Point(ActualWidth, ActualHeight);
                    break;
                case TickBarPlacement.Right:
                    getXFunc = (tickPosition, textWidth) => ActualWidth + tickTextSpacing;
                    getYFunc = (tickPosition, textHeight) => (IsDirectionReversed ? tickPosition - Minimum : Maximum - tickPosition) / (Maximum - Minimum) * ActualHeight - textHeight / 2;
                    guidelineSet = new GuidelineSet(new[] { -baseLinePen.Thickness / 2, baseLinePen.Thickness / 2 }, null);
                    lineStartPoint = new Point(0, 0);
                    lineEndPoint = new Point(0, ActualHeight);
                    break;
                case TickBarPlacement.Bottom:
                    getXFunc = (tickPosition, textWidth) => (IsDirectionReversed ? Maximum - tickPosition : tickPosition - Minimum) / (Maximum - Minimum) * ActualWidth - textWidth / 2;
                    getYFunc = (tickPosition, textHeight) => ActualHeight + tickTextSpacing;
                    guidelineSet = new GuidelineSet(null, new[] { -baseLinePen.Thickness / 2, baseLinePen.Thickness / 2 });
                    lineStartPoint = new Point(0, 0);
                    lineEndPoint = new Point(ActualWidth, 0);
                    break;
                default:
                    throw new InvalidEnumArgumentException(Placement.ToString());
            }

            var drawTickTextAction = new Action<double, Func<double, double, double>, Func<double, double, double>>((tickPosition, getX, getY) =>
            {
                string tickText = String.Format(TickStringFormat, tickPosition);
                var text = new FormattedText(tickText, culture, FlowDirection, typeface, fontSize, Fill);
                double x = getX(tickPosition, text.Width);
                double y = getY(tickPosition, text.Height);

                var textRect = new Rect(new Point(x, y), new Size(text.Width, text.Height));

                if (tickPosition == Maximum)
                {
                    MaximumLabelPosition = textRect;
                    if (IsMaximumVisible)
                    {
                        context.DrawText(text, new Point(x, y));
                    }
                }
                else if (tickPosition == Minimum)
                {
                    MinimumLabelPosition = textRect;
                    if (IsMinimumVisible)
                    {
                        context.DrawText(text, new Point(x, y));
                    }
                }
                else
                {
                    context.DrawText(text, new Point(x, y));
                }
            });

            // 给定 tick 绘制
            if (Ticks.Count > 0)
            {
                foreach (double tickPosition in Ticks)
                {
                    drawTickTextAction.Invoke(tickPosition, getXFunc, getYFunc);
                }
            }
            // 根据 tick frequency 自动计算 tick
            else
            {
                drawTickTextAction.Invoke(Maximum, getXFunc, getYFunc);

                _isTicksAmountChanged = true;
                TickFrequency = !_isTicksAmountSet ? TickFrequency : (Maximum - Minimum) / (TicksAmount - 1);

                if (TickFrequency != 0)
                {
                    // 当最后一个 tick 到最大值的间距小于 tick 间距的 0.15 倍时不显示该 tick 的刻度值文字
                    // 避免最后一个 tick 处刻度值文字过于拥挤
                    const double hideLastTickThreshold = 0.15;

                    for (double tickPosition = Minimum; tickPosition < Maximum; tickPosition += TickFrequency)
                    {
                        if (Maximum - tickPosition < TickFrequency * hideLastTickThreshold)
                            continue;

                        drawTickTextAction.Invoke(tickPosition, getXFunc, getYFunc);
                    }
                }
            }

            context.PushGuidelineSet(guidelineSet);
            {
                context.DrawLine(baseLinePen, lineStartPoint, lineEndPoint);
            }
            context.Pop();

            base.OnRender(context);
        }

        // 重写该方法并返回 null 可避免刻度值文字的显示被裁剪
        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            return null;
        }

        #region DependencyProperties

        #region TicksAmount : int

        public static readonly DependencyProperty TicksAmountProperty =
            DependencyProperty.Register(nameof(TicksAmount), typeof(int), typeof(ScaleBar),
                new PropertyMetadata(2,
                    (d, e) => (d as ScaleBar).OnTicksAmountPropertyChanged(),
                    (d, v) => (int)v >= 2 ? v : 2));

        public int TicksAmount
        {
            get { return (int)GetValue(TicksAmountProperty); }
            set { SetValue(TicksAmountProperty, value); }
        }

        private void OnTicksAmountPropertyChanged()
        {
            _isTicksAmountChanged = true;
            _isTicksAmountSet = true;
            TickFrequency = (Maximum - Minimum) / (TicksAmount - 1);
        }

        #endregion

        #region TickStringFormat : string

        public static readonly DependencyProperty TickStringFormatProperty =
            DependencyProperty.Register(nameof(TickStringFormat), typeof(string), typeof(ScaleBar),
                new PropertyMetadata("{0}", (d, e) => (d as ScaleBar).InvalidateVisual()));

        public string TickStringFormat
        {
            get { return (string)GetValue(TickStringFormatProperty); }
            set { SetValue(TickStringFormatProperty, value); }
        }

        #endregion

        #region TickStringVisible : bool

        public static readonly DependencyProperty TickStringVisibleProperty =
            DependencyProperty.Register(nameof(TickStringVisible), typeof(bool), typeof(ScaleBar),
                new PropertyMetadata(true, (d, e) => (d as ScaleBar).InvalidateVisual()));

        public bool TickStringVisible
        {
            get { return (bool)GetValue(TickStringVisibleProperty); }
            set { SetValue(TickStringVisibleProperty, value); }
        }

        #endregion

        #region IsMaximumVisible : bool

        internal static readonly DependencyProperty IsMaximumVisibleProperty =
            DependencyProperty.Register(nameof(IsMaximumVisible), typeof(bool), typeof(ScaleBar),
                new PropertyMetadata(true, (d, e) => (d as ScaleBar).InvalidateVisual()));

        internal bool IsMaximumVisible
        {
            get { return (bool)GetValue(IsMaximumVisibleProperty); }
            set { SetValue(IsMaximumVisibleProperty, value); }
        }

        #endregion

        #region IsMinimumVisible : bool

        internal static readonly DependencyProperty IsMinimumVisibleProperty =
            DependencyProperty.Register(nameof(IsMinimumVisible), typeof(bool), typeof(ScaleBar),
                new PropertyMetadata(true, (d, e) => (d as ScaleBar).InvalidateVisual()));

        internal bool IsMinimumVisible
        {
            get { return (bool)GetValue(IsMinimumVisibleProperty); }
            set { SetValue(IsMinimumVisibleProperty, value); }
        }

        #endregion

        #endregion

        #region LabelPosition

        private Rect _maximumLabelPosition;
        internal Rect MaximumLabelPosition
        {
            get { return _maximumLabelPosition; }
            private set
            {
                _maximumLabelPosition = value;
                RaiseLabelPositionChanged();
            }
        }

        private Rect _minimumLabelPosition;
        internal Rect MinimumLabelPosition
        {
            get { return _minimumLabelPosition; }
            private set
            {
                _minimumLabelPosition = value;
                RaiseLabelPositionChanged();
            }
        }

        internal event EventHandler<EventArgs> LabelPositionChanged;

        private void RaiseLabelPositionChanged()
        {
            var handler = LabelPositionChanged;
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
