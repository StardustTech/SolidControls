using System.Windows;

namespace Stardust.OpenSource.SolidControls.Demo.WpfApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static new App Current { get { return (Application.Current as App)!; } }
}
