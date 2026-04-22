using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AOOP3.Services;
using Avalonia.Platform.Storage;

namespace AOOP3.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IFlightService _flightService;
    private readonly IExportService _exportService;

    public RouteViewModel RouteViewModel { get; }
    public FlightInfoViewModel FlightInfoViewModel { get; }
    public AnalyticsViewModel AnalyticsViewModel { get; }

    [ObservableProperty]
    private ViewModelBase _currentViewModel;

    [ObservableProperty]
    private string _exportStatus = "Ready";

    public MainWindowViewModel(IFlightService flightService, IExportService exportService)
    {
        _flightService = flightService;
        _exportService = exportService;

        RouteViewModel = new RouteViewModel(_flightService);
        FlightInfoViewModel = new FlightInfoViewModel(_flightService);
        AnalyticsViewModel = new AnalyticsViewModel(_flightService);

        CurrentViewModel = RouteViewModel;
    }

    // Parameterless constructor for designer
    public MainWindowViewModel()
    {
        _flightService = new FlightService(new FlightRepository());
        _exportService = new ExportService();
        RouteViewModel = new RouteViewModel(_flightService);
        FlightInfoViewModel = new FlightInfoViewModel(_flightService);
        AnalyticsViewModel = new AnalyticsViewModel(_flightService);

        CurrentViewModel = RouteViewModel;
    }

    [RelayCommand]
    private void NavigateToRoute()
    {
        CurrentViewModel = RouteViewModel;
    }

    [RelayCommand]
    private void NavigateToFlightInfo()
    {
        CurrentViewModel = FlightInfoViewModel;
    }

    [RelayCommand]
    private void NavigateToAnalytics()
    {
        CurrentViewModel = AnalyticsViewModel;
    }

    [RelayCommand]
    private async Task ExportCsvAsync()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow != null)
        {
            var topLevel = TopLevel.GetTopLevel(desktop.MainWindow);
            if (topLevel == null) return;

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Export Flights as CSV",
                DefaultExtension = "csv",
                SuggestedFileName = "flights_export.csv",
                FileTypeChoices = new[] { new FilePickerFileType("CSV Files") { Patterns = new[] { "*.csv" } } }
            });

            if (file != null)
            {
                ExportStatus = "Exporting CSV...";
                var flights = _flightService.GetAllFlights();
                await _exportService.ExportFlightsCsvAsync(flights, file.Path.LocalPath);
                ExportStatus = $"Exported to {file.Name}";
            }
        }
    }

    [RelayCommand]
    private async Task ExportPdfAsync()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow != null)
        {
            var topLevel = TopLevel.GetTopLevel(desktop.MainWindow);
            if (topLevel == null) return;

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Export Analytics as TXT",
                DefaultExtension = "txt",
                SuggestedFileName = "analytics_export.txt",
                FileTypeChoices = new[] { new FilePickerFileType("Text Files") { Patterns = new[] { "*.txt" } } }
            });

            if (file != null)
            {
                ExportStatus = "Exporting Analytics...";
                var flights = _flightService.GetAllFlights();

                var topAirlines = flights.GroupBy(f => f.AirlineName)
                    .Select(g => new { Airline = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(5);

                var sb = new System.Text.StringBuilder();
                sb.AppendLine("--- Flight Tracker Analytics Report ---");
                sb.AppendLine($"Total Flights: {flights.Count()}");
                sb.AppendLine("\nTop 5 Airlines by Traffic:");
                foreach(var a in topAirlines)
                {
                    sb.AppendLine($"- {a.Airline}: {a.Count} flights");
                }

                await _exportService.ExportAnalyticsTxtAsync(sb.ToString(), file.Path.LocalPath);
                ExportStatus = $"Exported to {file.Name}";
            }
        }
    }
}
