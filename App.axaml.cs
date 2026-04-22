using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AOOP3.ViewModels;
using AOOP3.Views;
using AOOP3.Services;

namespace AOOP3;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var dataRepo = new FlightRepository();
            var flightService = new FlightService(dataRepo);
            var exportService = new ExportService();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(flightService, exportService),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
