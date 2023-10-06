using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Stardust.OpenSource.SolidControls.Wpf
{
    public class ResourceBinding : MarkupExtension
    {
        #region Helper properties

        public static object GetResourceBindingKeyHelper(DependencyObject obj) {
            return obj.GetValue(ResourceBindingKeyHelperProperty);
        }

        public static void SetResourceBindingKeyHelper(DependencyObject obj, object value) {
            obj.SetValue(ResourceBindingKeyHelperProperty, value);
        }

        private const string ResourceBindingKeyHelper = nameof(ResourceBindingKeyHelper);

        public static readonly DependencyProperty ResourceBindingKeyHelperProperty =
            DependencyProperty.RegisterAttached(ResourceBindingKeyHelper, typeof(object), typeof(ResourceBinding), new PropertyMetadata(null, ResourceKeyChanged));

        static void ResourceKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (!(d is FrameworkElement target))
                return;

            if (!(e.NewValue is Tuple<object, DependencyProperty> newVal))
                return;

            var dp = newVal.Item2;

            if (newVal.Item1 == null) {
                target.SetValue(dp, dp.GetMetadata(target).DefaultValue);
                return;
            }

            target.SetResourceReference(dp, newVal.Item1);
        }

        #endregion

        public ResourceBinding() { }

        public ResourceBinding(string path) {
            Path = new PropertyPath(path);
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget provideValueTargetService))
                return null;

            if (provideValueTargetService.TargetObject != null && provideValueTargetService.TargetObject.GetType().FullName == "System.Windows.SharedDp")
                return this;

            if (!(provideValueTargetService.TargetObject is FrameworkElement targetObject))
                return null;

            if (!(provideValueTargetService.TargetProperty is DependencyProperty targetProperty))
                return null;

            #region binding

            var binding = new Binding {
                Path = Path,
                XPath = XPath,

                Mode = Mode,
                UpdateSourceTrigger = UpdateSourceTrigger,
                FallbackValue = FallbackValue,

                Converter = Converter,
                ConverterParameter = ConverterParameter,
                ConverterCulture = ConverterCulture,
            };

            if (Source != null) {
                binding.Source = Source;
            }

            if (RelativeSource != null) {
                binding.RelativeSource = RelativeSource;
            }

            if (ElementName != null) {
                binding.ElementName = ElementName;
            }

            #endregion

            var multiBinding = new MultiBinding {
                Converter = HelperConverter.Instance,
                ConverterParameter = targetProperty
            };

            multiBinding.Bindings.Add(binding);
            multiBinding.NotifyOnSourceUpdated = true;

            targetObject.SetBinding(ResourceBindingKeyHelperProperty, multiBinding);

            return null;

        }

        #region Binding Members

        public object Source { get; set; }

        public PropertyPath Path { get; set; }

        [DefaultValue(null)]
        public string XPath { get; set; }

        [DefaultValue(BindingMode.Default)]
        public BindingMode Mode { get; set; }

        [DefaultValue(UpdateSourceTrigger.Default)]
        public UpdateSourceTrigger UpdateSourceTrigger { get; set; }

        [DefaultValue(null)]
        public IValueConverter Converter { get; set; }

        [DefaultValue(null)]
        public object ConverterParameter { get; set; }

        [DefaultValue(null)]
        [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
        public CultureInfo ConverterCulture { get; set; }

        [DefaultValue(null)]
        public RelativeSource RelativeSource { get; set; }

        [DefaultValue(null)]
        public string ElementName { get; set; }

        #endregion

        #region BindingBase Members

        public object FallbackValue { get; set; }

        #endregion

        #region Nested types

        private class HelperConverter : IMultiValueConverter
        {
            private static readonly Lazy<HelperConverter> _instance = new Lazy<HelperConverter>();
            public static IMultiValueConverter Instance { get { return _instance.Value; } }

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
                return Tuple.Create(values[0], (DependencyProperty)parameter);
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
