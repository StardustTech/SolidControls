using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Stardust.OpenSource.SolidControls.Wpf
{
    [TemplatePart(Name = PART_MaximumEditTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_MinimumEditTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_ScaleBar, Type = typeof(ScaleBar))]
    public sealed class EditableScaleBar : Control
    {
        #region Basic supports for CustomControl

        static EditableScaleBar() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EditableScaleBar), new FrameworkPropertyMetadata(typeof(EditableScaleBar)));
        }

        public const string PART_MaximumEditTextBox = nameof(PART_MaximumEditTextBox);
        public const string PART_MinimumEditTextBox = nameof(PART_MinimumEditTextBox);
        public const string PART_ScaleBar = nameof(PART_ScaleBar);

        private TextBox _maximumEditTextBox;
        private TextBox _minimumEditTextBox;
        private ScaleBar _scaleBar;

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            _maximumEditTextBox = GetTemplateChild(PART_MaximumEditTextBox) as TextBox;
            _minimumEditTextBox = GetTemplateChild(PART_MinimumEditTextBox) as TextBox;
            _scaleBar = GetTemplateChild(PART_ScaleBar) as ScaleBar;

            _scaleBar.LabelPositionChanged += ScaleBar_LabelPositionChanged;

            UpdateTextBoxBindings();

            _maximumEditTextBox.PreviewKeyDown += ValueEditTextBox_PreviewKeyDown;
            _minimumEditTextBox.PreviewKeyDown += ValueEditTextBox_PreviewKeyDown;

            _maximumEditTextBox.LostFocus += (s, e) => _maximumEditTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            _minimumEditTextBox.LostFocus += (s, e) => _minimumEditTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            var dpDescriptor = DependencyPropertyDescriptor.FromProperty(TickStringFormatProperty, typeof(EditableScaleBar));
            dpDescriptor.AddValueChanged(this, (s, e) => UpdateTextBoxBindings());
        }

        private void ValueEditTextBox_PreviewKeyDown(object sender, KeyEventArgs e) {
            var textBox = sender as TextBox;
            if (e.Key == Key.Enter) {
                textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
            else if (e.Key == Key.Escape) {
                textBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            }
        }

        private void UpdateTextBoxBindings() {
            if (_maximumEditTextBox == null || _minimumEditTextBox == null)
                return;

            _maximumEditTextBox.SetBinding(TextBox.TextProperty, new Binding() {
                Source = this,
                Path = new PropertyPath(nameof(Maximum)),
                StringFormat = TickStringFormat,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateScaleTrigger,
            });

            _minimumEditTextBox.SetBinding(TextBox.TextProperty, new Binding() {
                Source = this,
                Path = new PropertyPath(nameof(Minimum)),
                StringFormat = TickStringFormat,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateScaleTrigger,
            });
        }

        #endregion

        #region EventHandlers

        private void ScaleBar_LabelPositionChanged(object sender, EventArgs e) {
            var maximumRect = _scaleBar.MaximumLabelPosition;
            Canvas.SetLeft(_maximumEditTextBox, maximumRect.Left - maximumRect.Width / 4);
            Canvas.SetTop(_maximumEditTextBox, maximumRect.Top);
            _maximumEditTextBox.Width = maximumRect.Width * 1.5;

            var minimumRect = _scaleBar.MinimumLabelPosition;
            Canvas.SetLeft(_minimumEditTextBox, minimumRect.Left - minimumRect.Width / 4);
            Canvas.SetTop(_minimumEditTextBox, minimumRect.Top);
            _minimumEditTextBox.Width = minimumRect.Width * 1.5;
        }

        #endregion

        #region DependencyProperties

        #region Fill : Brush

        public static readonly DependencyProperty FillProperty =
            TickBar.FillProperty.AddOwner(typeof(EditableScaleBar));

        public Brush Fill {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        #endregion

        #region Minimum : double

        public static readonly DependencyProperty MinimumProperty =
            TickBar.MinimumProperty.AddOwner(typeof(EditableScaleBar));

        public double Minimum {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        #endregion

        #region Maximum : double

        public static readonly DependencyProperty MaximumProperty =
            TickBar.MaximumProperty.AddOwner(typeof(EditableScaleBar));

        public double Maximum {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        #endregion

        #region TickFrequency : double

        public static readonly DependencyProperty TickFrequencyProperty =
            TickBar.TickFrequencyProperty.AddOwner(typeof(EditableScaleBar));

        public double TickFrequency {
            get { return (double)GetValue(TickFrequencyProperty); }
            set { SetValue(TickFrequencyProperty, value); }
        }

        #endregion

        #region Ticks : DoubleCollection

        public static readonly DependencyProperty TicksProperty =
            TickBar.TicksProperty.AddOwner(typeof(EditableScaleBar));

        public DoubleCollection Ticks {
            get { return (DoubleCollection)GetValue(TicksProperty); }
            set { SetValue(TicksProperty, value); }
        }

        #endregion

        #region TicksAmount : int

        public static readonly DependencyProperty TicksAmountProperty =
            ScaleBar.TicksAmountProperty.AddOwner(typeof(EditableScaleBar));

        public int TicksAmount {
            get { return (int)GetValue(TicksAmountProperty); }
            set { SetValue(TicksAmountProperty, value); }
        }

        #endregion

        #region IsDirectionReversed : bool

        public static readonly DependencyProperty IsDirectionReversedProperty =
            TickBar.IsDirectionReversedProperty.AddOwner(typeof(EditableScaleBar));

        public bool IsDirectionReversed {
            get { return (bool)GetValue(IsDirectionReversedProperty); }
            set { SetValue(IsDirectionReversedProperty, value); }
        }

        #endregion

        #region Placement : TickBarPlacement

        public static readonly DependencyProperty PlacementProperty =
            TickBar.PlacementProperty.AddOwner(typeof(EditableScaleBar));

        public TickBarPlacement Placement {
            get { return (TickBarPlacement)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        #endregion

        #region TickStringFormat : string

        public static readonly DependencyProperty TickStringFormatProperty =
            ScaleBar.TickStringFormatProperty.AddOwner(typeof(EditableScaleBar));

        public string TickStringFormat {
            get { return (string)GetValue(TickStringFormatProperty); }
            set { SetValue(TickStringFormatProperty, value); }
        }

        #endregion

        #region TickStringVisible : bool

        public static readonly DependencyProperty TickStringVisibleProperty =
            ScaleBar.TickStringVisibleProperty.AddOwner(typeof(EditableScaleBar));

        public bool TickStringVisible {
            get { return (bool)GetValue(TickStringVisibleProperty); }
            set { SetValue(TickStringVisibleProperty, value); }
        }

        #endregion

        #region IsMaximumEditable : bool

        public static readonly DependencyProperty IsMaximumEditableProperty =
            DependencyProperty.Register(nameof(IsMaximumEditable), typeof(bool), typeof(EditableScaleBar),
                new PropertyMetadata(true, (d, e) => (d as EditableScaleBar).InvalidateVisual()));

        public bool IsMaximumEditable {
            get { return (bool)GetValue(IsMaximumEditableProperty); }
            set { SetValue(IsMaximumEditableProperty, value); }
        }

        #endregion

        #region IsMinimumEditable : bool

        public static readonly DependencyProperty IsMinimumEditableProperty =
            DependencyProperty.Register(nameof(IsMinimumEditable), typeof(bool), typeof(EditableScaleBar),
                new PropertyMetadata(true, (d, e) => (d as EditableScaleBar).InvalidateVisual()));

        public bool IsMinimumEditable {
            get { return (bool)GetValue(IsMinimumEditableProperty); }
            set { SetValue(IsMinimumEditableProperty, value); }
        }

        #endregion

        #region UpdateScaleTrigger : UpdateSourceTrigger

        public static readonly DependencyProperty UpdateScaleTriggerProperty =
            DependencyProperty.Register(nameof(UpdateScaleTrigger), typeof(UpdateSourceTrigger), typeof(EditableScaleBar),
                new PropertyMetadata(UpdateSourceTrigger.PropertyChanged, (d, e) => (d as EditableScaleBar).UpdateTextBoxBindings()));

        public UpdateSourceTrigger UpdateScaleTrigger {
            get { return (UpdateSourceTrigger)GetValue(UpdateScaleTriggerProperty); }
            set { SetValue(UpdateScaleTriggerProperty, value); }
        }

        #endregion

        #endregion
    }
}
