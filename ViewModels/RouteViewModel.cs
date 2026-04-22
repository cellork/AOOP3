using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Tiling;
using NetTopologySuite.Geometries;
using AOOP3.Models;
using AOOP3.Services;

namespace AOOP3.ViewModels;

public partial class RouteViewModel : ViewModelBase
{
    private readonly IFlightService _flightService;
    private readonly List<Airport> _allAirports = new();

    private readonly MemoryLayer _routeLayer;

    public Map Map { get; } = new Map();

    public ObservableCollection<Airport> FilteredAirports { get; } = new();

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private Airport? _selectedAirport;

    // Computed Properties
    public int DestinationCount { get; private set; }
    public int DailyFlightCount { get; private set; }

    public RouteViewModel(IFlightService flightService)
    {
        _flightService = flightService;

        // Initialize Map with OpenStreetMap tiles
        Map.Layers.Add(OpenStreetMap.CreateTileLayer());
        
        // Initialize dynamic route layer
        _routeLayer = new MemoryLayer { Name = "Routes" };
        Map.Layers.Add(_routeLayer);

        LoadAirports();
    }

    private void LoadAirports()
    {
        _allAirports.AddRange(_flightService.GetAllAirports());
        UpdateFilteredList(string.Empty);
    }

    partial void OnSearchQueryChanged(string value)
    {
        UpdateFilteredList(value);
    }

    private void UpdateFilteredList(string query)
    {
        FilteredAirports.Clear();

        var filtered = string.IsNullOrWhiteSpace(query)
            ? _allAirports
            : _allAirports.Where(a => 
                a.Name.Contains(query, StringComparison.OrdinalIgnoreCase) || 
                a.IataCode.Contains(query, StringComparison.OrdinalIgnoreCase));

        foreach (var airport in filtered)
        {
            FilteredAirports.Add(airport);
        }
    }

    partial void OnSelectedAirportChanged(Airport? value)
    {
        UpdateStats(value);
        DrawRoutesOnMap(value);
    }

    private void UpdateStats(Airport? airport)
    {
        if (airport == null)
        {
            DestinationCount = 0;
            DailyFlightCount = 0;
        }
        else
        {
            // Assuming IFlightService can get flights by departure IATA code
            var outboundFlights = _flightService.GetAllFlights()
                .Where(f => f.DepartureAirport == airport.IataCode)
                .ToList();

            DailyFlightCount = outboundFlights.Count;
            DestinationCount = outboundFlights.Select(f => f.ArrivalAirport).Distinct().Count();
        }

        OnPropertyChanged(nameof(DestinationCount));
        OnPropertyChanged(nameof(DailyFlightCount));
    }

    [RelayCommand]
    private void Clear()
    {
        SearchQuery = string.Empty; // Resets search (and auto-triggers list update)
        SelectedAirport = null;     // Resets selection (and auto-triggers map clear)
    }

    private void DrawRoutesOnMap(Airport? origin)
{
    var features = new List<IFeature>();

    if (origin != null)
    {
        // 1. Draw Origin Pin
        var (originX, originY) = SphericalMercator.FromLonLat(origin.Longitude, origin.Latitude);
        features.Add(new GeometryFeature
        {
            Geometry = new Point(originX, originY),
            Styles = new[] { new SymbolStyle { Fill = new Brush(Color.Red), SymbolScale = 0.8 } }
        });

        // 2. Find destinations and draw Lines & Destination Pins
        var outboundIatas = _flightService.GetAllFlights()
            .Where(f => f.DepartureAirport == origin.IataCode)
            .Select(f => f.ArrivalAirport)
            .Distinct();

        var destinations = _allAirports.Where(a => outboundIatas.Contains(a.IataCode));

        foreach (var dest in destinations)
        {
            var (destX, destY) = SphericalMercator.FromLonLat(dest.Longitude, dest.Latitude);

            // Add LineString (Route)
            features.Add(new GeometryFeature
            {
                Geometry = new LineString(new[] { new Coordinate(originX, originY), new Coordinate(destX, destY) }),
                Styles = new[] { new VectorStyle { Line = new Pen(Color.Gray, 2) { PenStyle = PenStyle.Dash } } }
            });

            // Add Destination Pin
            features.Add(new GeometryFeature
            {
                Geometry = new Point(destX, destY),
                Styles = new[] { new SymbolStyle { Fill = new Brush(Color.Blue), SymbolScale = 0.5 } }
            });
        }
    }

    _routeLayer.Features = features;
    _routeLayer.DataHasChanged();

    // 3. Pan and Zoom Map to fit the new routes
    if (features.Any())
    {
        var extent = _routeLayer.Extent;
        if (extent != null)
        {
            Map.Navigator.ZoomToBox(extent.Grow(extent.Width * 0.1));
        }
    }
}
}