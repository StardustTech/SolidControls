using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SolidControls
{
    internal sealed class LineChartKernel : Shape
    {
        #region Private Fields

        private readonly UIElementCollection _xSliders;
        private readonly UIElementCollection _ySliders;

        private Matrix _transformMatrix;
        private Matrix _invertMatrix;

        private ImageSource _dataImage;

        private double _zoomFactorX = 1;
        private double _zoomFactorY = 1;

        private double _dataMinX;
        private double _dataMaxX;
        private double _dataMinY;
        private double _dataMaxY;

        private Point _lastMousePosition;
        private LineChartSlider _currentSlider;
        private bool _currentSliderIsXSlider;
        private RangeSliderPosition? _rangeSliderPosition;

        #endregion

        #region Properties

        public UIElementCollection XSliders { get { return _xSliders; } }
        public UIElementCollection YSliders { get { return _ySliders; } }

        #endregion

        public LineChartKernel()
        {
            _xSliders = new UIElementCollection(this, this);
            _ySliders = new UIElementCollection(this, this);

            var sliderUpdatedEvent = LineChartSlider.SliderUpdatedEvent.AddOwner(typeof(LineChartKernel));
            AddHandler(sliderUpdatedEvent, new RoutedEventHandler((s, e) => InvalidateVisual()));
        }

        #region DependencyProperties

        #region Points : PointCollection

        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(nameof(Points), typeof(PointCollection), typeof(LineChartKernel),
                new UIPropertyMetadata(null, (d, e) => (d as LineChartKernel).InvalidatePoints()));

        #endregion

        #region MinX : double

        public double MinX
        {
            get { return (double)GetValue(MinXProperty); }
            set { SetValue(MinXProperty, value); }
        }

        public static readonly DependencyProperty MinXProperty =
            DependencyProperty.Register(nameof(MinX), typeof(double), typeof(LineChartKernel),
                new UIPropertyMetadata(0.0, (d, e) => (d as LineChartKernel).InvalidateTransformMatrix()));

        #endregion

        #region MaxX : double

        public double MaxX
        {
            get { return (double)GetValue(MaxXProperty); }
            set { SetValue(MaxXProperty, value); }
        }

        public static readonly DependencyProperty MaxXProperty =
            DependencyProperty.Register(nameof(MaxX), typeof(double), typeof(LineChartKernel),
                new UIPropertyMetadata(100.0,
                    (d, e) => (d as LineChartKernel).InvalidateTransformMatrix()));

        #endregion

        #region MinY : double

        public double MinY
        {
            get { return (double)GetValue(MinYProperty); }
            set { SetValue(MinYProperty, value); }
        }

        public static readonly DependencyProperty MinYProperty =
            DependencyProperty.Register(nameof(MinY), typeof(double), typeof(LineChartKernel),
                new UIPropertyMetadata(0.0, (d, e) => (d as LineChartKernel).InvalidateTransformMatrix()));

        #endregion

        #region MaxY : double

        public double MaxY
        {
            get { return (double)GetValue(MaxYProperty); }
            set { SetValue(MaxYProperty, value); }
        }

        public static readonly DependencyProperty MaxYProperty =
            DependencyProperty.Register(nameof(MaxY), typeof(double), typeof(LineChartKernel),
                new UIPropertyMetadata(100.0, (d, e) => (d as LineChartKernel).InvalidateTransformMatrix()));

        #endregion

        #region AutoResetZoomX : bool

        public bool AutoResetZoomX
        {
            get { return (bool)GetValue(AutoResetZoomXProperty); }
            set { SetValue(AutoResetZoomXProperty, value); }
        }

        public static readonly DependencyProperty AutoResetZoomXProperty =
            DependencyProperty.Register(nameof(AutoResetZoomX), typeof(bool), typeof(LineChartKernel),
                new UIPropertyMetadata(true, (d, e) => (d as LineChartKernel).InvalidateTransformMatrix(true)));

        #endregion

        #region AutoResetZoomY : bool

        public bool AutoResetZoomY
        {
            get { return (bool)GetValue(AutoResetZoomYProperty); }
            set { SetValue(AutoResetZoomYProperty, value); }
        }

        public static readonly DependencyProperty AutoResetZoomYProperty =
            DependencyProperty.Register(nameof(AutoResetZoomY), typeof(bool), typeof(LineChartKernel),
                new UIPropertyMetadata(true, (d, e) => (d as LineChartKernel).InvalidateTransformMatrix(true)));

        #endregion

        #endregion

        #region Overrides

        protected override Geometry DefiningGeometry { get { return new LineGeometry(); } }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (_dataImage == null)
                return;

            drawingContext.DrawImage(_dataImage, new Rect(new Point(), RenderSize));

            const double sliderThickness = 2;
            const double rangeSliderOpacity = 0.2;

            foreach (object sliderChild in XSliders)
            {
                if (!(sliderChild is LineChartSlider slider))
                    continue;

                if (slider.Value.IsBetween(MinX, MaxX))
                {
                    var startPoint = new Point(slider.Value, MinY);
                    var endPoint = new Point(slider.Value, MaxY);
                    var line = new LineGeometry(startPoint, endPoint)
                    {
                        Transform = new MatrixTransform(_transformMatrix),
                    };
                    drawingContext.DrawGeometry(Brushes.Transparent, new Pen(new SolidColorBrush(slider.Color), sliderThickness), line);
                }

                if (slider is LineChartRangeSlider rangeSlider)
                {
                    if (rangeSlider.EndValue.IsBetween(MinX, MaxX))
                    {
                        var startPoint = new Point(rangeSlider.EndValue, MinY);
                        var endPoint = new Point(rangeSlider.EndValue, MaxY);
                        var line = new LineGeometry(startPoint, endPoint)
                        {
                            Transform = new MatrixTransform(_transformMatrix),
                        };
                        drawingContext.DrawGeometry(Brushes.Transparent, new Pen(new SolidColorBrush(slider.Color), sliderThickness), line);
                    }

                    if ((rangeSlider.Value > MinX || rangeSlider.EndValue > MinX) && (rangeSlider.Value < MaxX || rangeSlider.EndValue < MaxX))
                    {
                        var topLeftPoint = new Point(Math.Max(Math.Min(rangeSlider.Value, rangeSlider.EndValue), MinX), MaxY);
                        var bottomRightPoint = new Point(Math.Min(Math.Max(rangeSlider.Value, rangeSlider.EndValue), MaxX), MinY);
                        var rect = new RectangleGeometry(new Rect(topLeftPoint, bottomRightPoint))
                        {
                            Transform = new MatrixTransform(_transformMatrix),
                        };
                        drawingContext.DrawGeometry(new SolidColorBrush(rangeSlider.Color) { Opacity = rangeSliderOpacity }, new Pen(Brushes.Transparent, 0), rect);
                    }
                }
            }

            foreach (object sliderChild in YSliders)
            {
                if (!(sliderChild is LineChartSlider slider))
                    continue;

                if (slider.Value.IsBetween(MinY, MaxY))
                {
                    var startPoint = new Point(MinX, slider.Value);
                    var endPoint = new Point(MaxX, slider.Value);
                    var line = new LineGeometry(startPoint, endPoint)
                    {
                        Transform = new MatrixTransform(_transformMatrix)
                    };
                    drawingContext.DrawGeometry(Brushes.Transparent, new Pen(new SolidColorBrush(slider.Color), sliderThickness), line);
                }

                if (slider is LineChartRangeSlider rangeSlider)
                {
                    if (rangeSlider.EndValue.IsBetween(MinY, MaxY))
                    {
                        var startPoint = new Point(MinX, rangeSlider.EndValue);
                        var endPoint = new Point(MaxX, rangeSlider.EndValue);
                        var line = new LineGeometry(startPoint, endPoint)
                        {
                            Transform = new MatrixTransform(_transformMatrix)
                        };
                        drawingContext.DrawGeometry(Brushes.Transparent, new Pen(new SolidColorBrush(slider.Color), sliderThickness), line);
                    }

                    if ((rangeSlider.Value > MinY || rangeSlider.EndValue > MinY) && (rangeSlider.Value < MaxY || rangeSlider.EndValue < MaxY))
                    {
                        var topLeftPoint = new Point(MinX, Math.Min(Math.Max(rangeSlider.Value, rangeSlider.EndValue), MaxY));
                        var bottomRightPoint = new Point(MaxX, Math.Max(Math.Min(rangeSlider.Value, rangeSlider.EndValue), MinY));
                        var rect = new RectangleGeometry(new Rect(topLeftPoint, bottomRightPoint))
                        {
                            Transform = new MatrixTransform(_transformMatrix),
                        };
                        drawingContext.DrawGeometry(new SolidColorBrush(rangeSlider.Color) { Opacity = rangeSliderOpacity }, new Pen(Brushes.Transparent, 0), rect);
                    }
                }
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (RenderSize.Height <= 0 || RenderSize.Width <= 0)
                return;

            InvalidateTransformMatrix();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            const double zoomSpeed = 0.05;

            // horizontal
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                double newZoomFactorX = _zoomFactorX * (1 + zoomSpeed * Math.Sign(e.Delta));

                if (newZoomFactorX < 1)
                {
                    newZoomFactorX = 1;
                }

                double zoomCenterX = _invertMatrix.Transform(e.GetPosition(this)).X;
                double dataRange = _dataMaxX - _dataMinX;
                double newMinX = zoomCenterX - dataRange * (zoomCenterX - MinX) / newZoomFactorX / (MaxX - MinX);
                double newMaxX = dataRange / newZoomFactorX + newMinX;

                if (newMaxX <= newMinX)
                    return;

                if (newMinX < _dataMinX)
                {
                    newMaxX += _dataMinX - newMinX;
                    newMinX = _dataMinX;
                }
                else if (newMaxX > _dataMaxX)
                {
                    newMinX -= newMaxX - _dataMaxX;
                    newMaxX = _dataMaxX;
                }

                _zoomFactorX = newZoomFactorX;

                InvalidateTransformMatrix(newMinX, newMaxX, MinY, MaxY);
            }
            // vertical
            else if (Keyboard.Modifiers == ModifierKeys.Alt)
            {
                double newZoomFactorY = _zoomFactorY * (1 + zoomSpeed * Math.Sign(e.Delta));

                if (newZoomFactorY < 1)
                {
                    newZoomFactorY = 1;
                }

                double zoomCenterY = _invertMatrix.Transform(e.GetPosition(this)).Y;
                double dataRange = _dataMaxY - _dataMinY;
                double newMinY = zoomCenterY - dataRange * (zoomCenterY - MinY) / newZoomFactorY / (MaxY - MinY);
                double newMaxY = dataRange / newZoomFactorY + newMinY;

                if (newMaxY <= newMinY)
                    return;

                if (newMinY < _dataMinY)
                {
                    newMaxY += _dataMinY - newMinY;
                    newMinY = _dataMinY;
                }
                else if (newMaxY > _dataMaxY)
                {
                    newMinY -= newMaxY - _dataMaxY;
                    newMaxY = _dataMaxY;
                }

                _zoomFactorY = newZoomFactorY;

                InvalidateTransformMatrix(MinX, MaxX, newMinY, newMaxY);
            }

            base.OnMouseWheel(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _currentSlider = GetNearestSlider(e.GetPosition(this), out _currentSliderIsXSlider, out _rangeSliderPosition);

            InvalidateMouseCursor();

            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _currentSlider = GetNearestSlider(e.GetPosition(this), out _currentSliderIsXSlider, out _rangeSliderPosition);

            InvalidateMouseCursor();

            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_dataImage == null)
                return;

            var currentPosition = e.GetPosition(this);

            if (e.LeftButton == MouseButtonState.Released)
            {
                _currentSlider = GetNearestSlider(e.GetPosition(this), out _currentSliderIsXSlider, out _rangeSliderPosition);
            }

            InvalidateMouseCursor();

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (_currentSlider != null)
                {
                    PerformSliderDrag(_invertMatrix.Transform(currentPosition));
                }
                else
                {
                    PerformImageDrag(_invertMatrix.Transform(currentPosition));
                }
            }

            _lastMousePosition = currentPosition;
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            _zoomFactorX = _zoomFactorY = 1;

            InvalidateTransformMatrix(true);
        }

        #endregion

        #region Private Methods

        private void InvalidatePoints()
        {
            if (Points != null && !Points.IsFrozen)
            {
                Points.Freeze();
            }

            bool isDataValid = Points != null && Points.Count > 0;
            if (isDataValid)
            {
                _dataMinX = Points.Min(p => p.X);
                _dataMaxX = Points.Max(p => p.X);
                _dataMinY = Points.Min(p => p.Y);
                _dataMaxY = Points.Max(p => p.Y);
                isDataValid &= _dataMaxX != _dataMinX;
            }
            else
            {
                _dataMinX = _dataMaxX = 0;
                _dataMinY = _dataMaxY = 0;
            }

            if (!isDataValid)
            {
                _dataImage = null;
                InvalidateVisual();
                return;
            }

            if (_dataImage == null)
            {
                InvalidateTransformMatrix(true);
                return;
            }

            double minY = Points.Min(p => p.Y);
            double maxY = Points.Max(p => p.Y);
            if (minY == maxY)
            {
                InvalidateTransformMatrix(Math.Max(_dataMinX, MinX), Math.Min(_dataMaxX, MaxX), minY - 1, maxY + 1);
            }
            else
            {
                if (AutoResetZoomX)
                {
                    MaxX = _dataMaxX;
                    MinX = _dataMinX;
                }

                if (AutoResetZoomY)
                {
                    MaxY = _dataMaxY;
                    MinY = _dataMinY;
                }

                InvalidateTransformMatrix(Math.Max(_dataMinX, MinX), Math.Min(_dataMaxX, MaxX), Math.Max(_dataMinY, MinY), Math.Min(_dataMaxY, MaxY));
            }
        }

        private void InvalidateTransformMatrix(bool reset = false)
        {
            if (reset)
            {
                InvalidateTransformMatrix(_dataMinX, _dataMaxX, _dataMinY, _dataMaxY);
            }
            else
            {
                InvalidateTransformMatrix(MinX, MaxX, MinY, MaxY, false);
            }
        }

        private void InvalidateTransformMatrix(double minX, double maxX, double minY, double maxY, bool updateRangeValues = true)
        {
            if (minX < _dataMinX)
            {
                minX = _dataMinX;
            }

            if (maxX > _dataMaxX)
            {
                maxX = _dataMaxX;
            }

            if (minX > maxX)
            {
                minX = maxX;
            }

            if (minY > maxY)
            {
                minY = maxY;
            }

            if (minX == maxX || minY == maxY)
            {
                _dataImage = null;
                InvalidateVisual();
                return;
            }

            double scaleX = RenderSize.Width / (maxX - minX);
            double scaleY = -RenderSize.Height / (maxY - minY);

            var matrix = Matrix.Identity;
            matrix.Scale(scaleX, scaleY);
            matrix.OffsetX = -RenderSize.Width * minX / (maxX - minX);
            matrix.OffsetY = RenderSize.Height * maxY / (maxY - minY);

            if (matrix.HasInverse)
            {
                _transformMatrix = matrix;
                _invertMatrix = _transformMatrix;
                _invertMatrix.Invert();
            }

            if (updateRangeValues)
            {
                MinX = minX;
                MaxX = maxX;
                MinY = minY;
                MaxY = maxY;
            }

            InvalidateDataImage();
        }

        private void InvalidateDataImage()
        {
            int width = (int)RenderSize.Width;
            int height = (int)RenderSize.Height;

            if (width <= 0 || height <= 0)
                return;

            if (Points == null || Points.Count == 0)
            {
                _dataImage = null;
                InvalidateVisual();
                return;
            }

            double scaleMinX = MinX, scaleMaxX = MaxX;

            var orderedPoints = Points.OrderBy(p => p.X).ToList();
            var dataPoints = orderedPoints.Where(p => p.X >= scaleMinX && p.X <= scaleMaxX).ToList();

            if (dataPoints.Count != 0)
            {
                if (dataPoints.Where(p => p.X == scaleMinX).Count() == 0)
                {
                    var preMinPoints = orderedPoints.Where(p => p.X < scaleMinX);
                    if (preMinPoints.Count() != 0)
                    {
                        var preMinPoint = preMinPoints.Last();
                        var postMinPoint = dataPoints.OrderBy(p => p.X).First();
                        if (scaleMinX >= _dataMinX && scaleMinX <= _dataMaxX)
                        {
                            dataPoints.Insert(0, GetPointOnLine(preMinPoint, postMinPoint, scaleMinX));
                        }
                    }
                }

                if (dataPoints.Where(p => p.X == scaleMaxX).Count() == 0)
                {
                    var postMaxPoints = orderedPoints.Where(p => p.X > scaleMaxX);
                    if (postMaxPoints.Count() != 0)
                    {
                        var postMaxPoint = postMaxPoints.First();
                        var preMaxPoint = dataPoints.OrderBy(p => p.X).Last();
                        if (scaleMaxX >= _dataMinX && scaleMaxX <= _dataMaxX)
                        {
                            dataPoints.Add(GetPointOnLine(preMaxPoint, postMaxPoint, scaleMaxX));
                        }
                    }
                }
            }
            else
            {
                var preMinPoints = orderedPoints.Where(p => p.X <= scaleMinX);
                var postMaxPoints = orderedPoints.Where(p => p.X >= scaleMaxX);
                if (preMinPoints.Count() == 0 || postMaxPoints.Count() == 0)
                {
                    _dataImage = null;
                    InvalidateVisual();
                    return;
                }

                if (scaleMinX >= _dataMinX && scaleMinX <= _dataMaxX)
                {
                    dataPoints.Insert(0, GetPointOnLine(preMinPoints.Last(), postMaxPoints.First(), scaleMinX));
                }

                if (scaleMaxX >= _dataMinX && scaleMaxX <= _dataMaxX)
                {
                    dataPoints.Add(GetPointOnLine(preMinPoints.Last(), postMaxPoints.First(), scaleMaxX));
                }
            }

            var points = dataPoints.Select(p => _transformMatrix.Transform(p)).ToList();

            var color = (Stroke as SolidColorBrush).Color;

            Task.Factory.StartNew(() =>
            {
                points.Sort(new Comparison<Point>((p1, p2) => Math.Sign(p1.X - p2.X)));

                byte[] buffer = new byte[width * height * 4];

                Parallel.For(1, points.Count, i =>
                {
                    LineGeneratorUtils.BresenhamLineGenerator(points[i - 1], points[i], (x, y) =>
                    {
                        if (x < 0 || x >= width || y < 0 || y >= height)
                            return;

                        buffer[(y * width + x) * 4 + 0] = color.B;
                        buffer[(y * width + x) * 4 + 1] = color.G;
                        buffer[(y * width + x) * 4 + 2] = color.R;
                        buffer[(y * width + x) * 4 + 3] = 0xFF;
                    });
                });

                var bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

                bitmap.Lock();

                var handle = new GCHandle();
                try
                {
                    handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    NativeMethods.CopyMemory(bitmap.BackBuffer, handle.AddrOfPinnedObject(), (uint)buffer.Length);
                }
                finally
                {
                    handle.Free();
                }

                bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
                bitmap.Unlock();
                bitmap.Freeze();

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    _dataImage = bitmap;
                    InvalidateVisual();
                }));
            });
        }

        private Point GetPointOnLine(Point lineStartPoint, Point lineEndPoint, double x)
        {
            if (x < lineStartPoint.X || x > lineEndPoint.X)
                throw new ArgumentOutOfRangeException("x");

            if (lineStartPoint.X == lineEndPoint.X)
                return new Point(x, (lineStartPoint.Y + lineEndPoint.Y) / 2);

            if (x == lineStartPoint.X)
                return lineStartPoint;

            if (x == lineEndPoint.X)
                return lineEndPoint;

            double y = lineStartPoint.Y + (x - lineStartPoint.X) / (lineEndPoint.X - lineStartPoint.X) * (lineEndPoint.Y - lineStartPoint.Y);
            return new Point(x, y);
        }

        private void PerformSliderDrag(Point transformedMousePosition)
        {
            if (_currentSlider == null)
                return;

            if (_currentSlider.IsReadOnly)
                return;

            if (!(_currentSlider is LineChartRangeSlider rangeSlider) || _rangeSliderPosition == RangeSliderPosition.Start)
            {
                _currentSlider.Value = _currentSliderIsXSlider ? transformedMousePosition.X : transformedMousePosition.Y;
            }
            else if (_rangeSliderPosition == RangeSliderPosition.Body)
            {
                var dataVector = transformedMousePosition - _invertMatrix.Transform(_lastMousePosition);
                double delta = _currentSliderIsXSlider ? dataVector.X : dataVector.Y;
                rangeSlider.Value += delta;
                rangeSlider.EndValue += delta;
            }
            else if (_rangeSliderPosition == RangeSliderPosition.End)
            {
                rangeSlider.EndValue = _currentSliderIsXSlider ? transformedMousePosition.X : transformedMousePosition.Y;
            }

            InvalidateVisual();
        }

        private void PerformImageDrag(Point transformedMousePosition)
        {
            double newMinX = MinX, newMaxX = MaxX, newMinY = MinY, newMaxY = MaxY;
            var dataVector = transformedMousePosition - _invertMatrix.Transform(_lastMousePosition);

            {
                double xOffset = dataVector.X;

                newMinX -= xOffset;
                if (_dataMinX > newMinX)
                {
                    newMaxX += _dataMinX - newMinX;
                    newMinX = _dataMinX;
                }

                newMaxX -= xOffset;
                if (_dataMaxX < newMaxX)
                {
                    newMinX -= newMaxX - _dataMaxX;
                    newMaxX = _dataMaxX;
                }
            }

            {
                double yOffset = dataVector.Y;

                newMinY -= yOffset;
                if (_dataMinY > newMinY)
                {
                    newMaxY += _dataMinY - newMinY;
                    newMinY = _dataMinY;
                }

                newMaxY -= yOffset;
                if (_dataMaxY < newMaxY)
                {
                    newMinY -= newMaxY - _dataMaxY;
                    newMaxY = _dataMaxY;
                }
            }

            InvalidateTransformMatrix(newMinX, newMaxX, newMinY, newMaxY);
        }

        private enum RangeSliderPosition { Start, Body, End }

        private LineChartSlider GetNearestSlider(Point position, out bool isXSlider, out RangeSliderPosition? rangeSliderPosition)
        {
            const double sliderTouchingDistance = 2;

            var dataPosition = _invertMatrix.Transform(position);
            RangeSliderPosition? rangeSliderPositionResult = null;

            LineChartSlider nearestSlider = null;

            foreach (object sliderChild in _xSliders)
            {
                if (!(sliderChild is LineChartSlider slider))
                    continue;

                var dataPoint = new Point(slider.Value, dataPosition.Y);
                if (Math.Abs(_transformMatrix.Transform(dataPoint).X - position.X) <= sliderTouchingDistance)
                {
                    if (slider is LineChartRangeSlider)
                    {
                        rangeSliderPositionResult = RangeSliderPosition.Start;
                    }
                    nearestSlider = slider;
                    break;
                }

                if (!(slider is LineChartRangeSlider))
                    continue;

                var rangeSlider = slider as LineChartRangeSlider;
                dataPoint = new Point(rangeSlider.EndValue, dataPosition.Y);
                if (Math.Abs(_transformMatrix.Transform(dataPoint).X - position.X) <= sliderTouchingDistance)
                {
                    rangeSliderPositionResult = RangeSliderPosition.End;
                    nearestSlider = slider;
                    break;
                }

                if (dataPosition.X.IsBetween(rangeSlider.Value, rangeSlider.EndValue))
                {
                    rangeSliderPositionResult = RangeSliderPosition.Body;
                    nearestSlider = slider;
                    break;
                }
            }

            isXSlider = nearestSlider != null;

            if (isXSlider)
            {
                rangeSliderPosition = rangeSliderPositionResult;
                return nearestSlider;
            }

            nearestSlider = null;

            foreach (object sliderChild in _ySliders)
            {
                if (!(sliderChild is LineChartSlider slider))
                    continue;

                var dataPoint = new Point(dataPosition.X, slider.Value);
                if (Math.Abs(_transformMatrix.Transform(dataPoint).Y - position.Y) <= sliderTouchingDistance)
                {
                    if (slider is LineChartRangeSlider)
                    {
                        rangeSliderPositionResult = RangeSliderPosition.Start;
                    }
                    nearestSlider = slider;
                    break;
                }

                if (!(slider is LineChartRangeSlider))
                    continue;

                var rangeSlider = slider as LineChartRangeSlider;
                dataPoint = new Point(dataPosition.X, rangeSlider.EndValue);
                if (Math.Abs(_transformMatrix.Transform(dataPoint).Y - position.Y) <= sliderTouchingDistance)
                {
                    rangeSliderPositionResult = RangeSliderPosition.End;
                    nearestSlider = slider;
                    break;
                }

                if (dataPosition.Y.IsBetween(rangeSlider.Value, rangeSlider.EndValue))
                {
                    rangeSliderPositionResult = RangeSliderPosition.Body;
                    nearestSlider = slider;
                    break;
                }
            }

            rangeSliderPosition = rangeSliderPositionResult;
            return nearestSlider;
        }

        private void InvalidateMouseCursor()
        {
            if (_currentSlider == null)
            {
                if (_zoomFactorX == 1 && _zoomFactorY == 1)
                {
                    Cursor = Cursors.Arrow;
                }
                else if (_zoomFactorX == 1)
                {
                    Cursor = Cursors.ScrollNS;
                }
                else if (_zoomFactorY == 1)
                {
                    Cursor = Cursors.ScrollWE;
                }
                else
                {
                    Cursor = Cursors.ScrollAll;
                }
            }
            else if (_currentSlider.IsReadOnly)
            {
                Cursor = Cursors.No;
            }
            else if (_rangeSliderPosition != RangeSliderPosition.Body)
            {
                Cursor = _currentSliderIsXSlider ? Cursors.SizeWE : Cursors.SizeNS;
            }
            else /* rangeSliderPosition == RangeSliderPosition.Body */
            {
                Cursor = _currentSliderIsXSlider ? Cursors.ScrollWE : Cursors.ScrollNS;
            }
        }

        #endregion
    }
}
