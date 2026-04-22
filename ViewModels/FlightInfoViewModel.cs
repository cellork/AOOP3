using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using AOOP3.Models;
using AOOP3.Services;

namespace AOOP3.ViewModels;

public partial class FlightInfoViewModel : ViewModelBase
{
    private readonly IFlightService _flightService;

    public ObservableCollection<Airport> FilteredAirports { get; } = new();
    public ObservableCollection<Flight> FilteredFlights { get; } = new();
    public ObservableCollection<string> StatusOptions { get; } = new();

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private Airport? _selectedAirport;

    [ObservableProperty]
    private string _selectedStatus = "All statuses";

    public string FlightsSummary => $"Showing {FilteredFlights.Count} flights";

    public FlightInfoViewModel(IFlightService flightService)
    {
        _flightService = flightService;

        // Populate Statuses
        StatusOptions.Add("All statuses");
        foreach (var status in Enum.GetValues<FlightStatus>())
        {
            StatusOptions.Add(status.ToString());
        }

        UpdateAirportList(string.Empty);
    }

    // Parameterless constructor for designer
    public FlightInfoViewModel()
    {
        _flightService = new FlightService(new FlightRepository());
    }

    partial void OnSearchQueryChanged(string value)
    {
        UpdateAirportList(value);
    }

    private void UpdateAirportList(string query)
    {
        FilteredAirports.Clear();
        var allAirports = _flightService.GetAllAirports();

        var filtered = string.IsNullOrWhiteSpace(query)
            ? allAirports
            : allAirports.Where(a =>
                a.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                a.IataCode.Contains(query, StringComparison.OrdinalIgnoreCase));

        foreach (var airport in filtered)
        {
            FilteredAirports.Add(airport);
        }
    }

    partial void OnSelectedAirportChanged(Airport? value)
    {
        UpdateFlightList();
    }

    partial void OnSelectedStatusChanged(string value)
    {
        UpdateFlightList();
    }

    private void UpdateFlightList()
    {
        FilteredFlights.Clear();
        if (SelectedAirport == null)
        {
            OnPropertyChanged(nameof(FlightsSummary));
            return;
        }

        var allFlights = _flightService.GetAllFlights()
            .Where(f => f.DepartureAirport == SelectedAirport.IataCode);

        if (SelectedStatus != "All statuses" && Enum.TryParse<FlightStatus>(SelectedStatus, out var statusEnum))
        {
            allFlights = allFlights.Where(f => f.Status == statusEnum);
        }

        foreach (var flight in allFlights)
        {
            FilteredFlights.Add(flight);
        }

        OnPropertyChanged(nameof(FlightsSummary));
    }
}
