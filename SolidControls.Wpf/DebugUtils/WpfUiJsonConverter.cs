//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;

namespace Stardust.OpenSource.SolidControls.Wpf.DebugUtils
{
    //public class TypeJsonConverter : JsonConverter<Type>
    //{
    //    public override Type Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
    //        throw new NotImplementedException();
    //    }

    //    public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options) {
    //        writer.WriteStringValue(value.FullName);
    //    }
    //}

    //public class DependencyObjectJsonConverter : JsonConverter<DependencyObject>
    //{
    //    public override UIElement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
    //        throw new NotImplementedException();
    //    }

    //    public override void Write(Utf8JsonWriter writer, DependencyObject value, JsonSerializerOptions options) {
    //        writer.WriteStringValue(value.GetType().Name);
    //    }
    //}

    //public class WpfUiJsonConverter : JsonConverterFactory
    //{
    //    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) {
    //        if (typeToConvert == typeof(Type))
    //            return new TypeJsonConverter();

    //        if (typeToConvert == typeof(DependencyObject) || typeToConvert.IsSubclassOf(typeof(DependencyObject)))
    //            return new DependencyObjectJsonConverter();

    //        return null;
    //    }

    //    public override bool CanConvert(Type typeToConvert) {
    //        return typeToConvert == typeof(Type) || typeToConvert == typeof(DependencyObject) || typeToConvert.IsSubclassOf(typeof(DependencyObject));
    //    }
    //}

    //internal class Test
    //{
    //    private readonly Queue<RoutedEventArgs> _reaQueue = new();

    //    public void RegisterRoutedEvents(UIElement uiElement) {
    //        var events = uiElement.GetType().GetEvents().Where(ei => {
    //            var eventInvokerParas = ei.EventHandlerType.GetMethod("Invoke").GetParameters();
    //            return eventInvokerParas.Length == 2 && (eventInvokerParas[1].ParameterType == typeof(RoutedEventArgs) || eventInvokerParas[1].ParameterType.IsSubclassOf(typeof(RoutedEventArgs)));
    //        });

    //        foreach (var ei in events) {
    //            var h = Delegate.CreateDelegate(ei.EventHandlerType, typeof(MainWindow).GetMethod(nameof(OnRoutedEvent), BindingFlags.Public));
    //            ei.AddEventHandler(uiElement, h);
    //        }
    //    }

    //    public void OnRoutedEvent(object _, dynamic e) {
    //        var rea = e as RoutedEventArgs;

    //        if (rea.RoutedEvent.Name.EndsWith("MouseMove") || rea.RoutedEvent.Name == "QueryCursor"
    //            || rea.RoutedEvent.Name.EndsWith("KeyboardFocus") || rea.RoutedEvent.Name.EndsWith("MouseCapture")) {
    //            return;
    //        }

    //        _reaQueue.Enqueue(rea);
    //    }

    //    public void SaveTo(string filepath) {
    //        string jsonText = JsonSerializer.Serialize(_reaQueue, new JsonSerializerOptions() {
    //            Converters = {
    //            new JsonStringEnumConverter(),
    //            new WpfUiJsonConverter()
    //        }
    //        });

    //        File.WriteAllText(filepath, jsonText);
    //    }
    //}
}
