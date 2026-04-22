using System.Collections.Generic;
using System.Linq;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Measure;
using SkiaSharp;
using AOOP3.Models;
using AOOP3.Services;
using AOOP3.Queries;

namespace AOOP3.ViewModels;

public partial class AnalyticsViewModel : ViewModelBase
{
    private readonly IFlightService _flightService;

    public ISeries[] BusiestRoutesSeries { get; set; } = new ISeries[0];
    public Axis[] BusiestRoutesYAxes { get; set; } = new Axis[0];
    public Axis[] BusiestRoutesXAxes { get; set; } = new Axis[0];

    public ISeries[] TopAirlinesSeries { get; set; } = new ISeries[0];

    public ISeries[] TrafficSeries { get; set; } = new ISeries[0];
    public Axis[] TrafficXAxes { get; set; } = new Axis[0];
    public Axis[] TrafficYAxes { get; set; } = new Axis[0];

    public AnalyticsViewModel(IFlightService flightService)
    {
        _flightService = flightService;
        LoadData();
    }

    public AnalyticsViewModel()
    {
        _flightService = new FlightService(new FlightRepository());
        LoadData();
    }

    private void LoadData()
    {
        var flights = _flightService.GetAllFlights().ToList();

        // 1. Busiest Routes
        var topRoutes = RouteQueries.GetBusiestRoutes(flights).ToList();

        var routesLabels = topRoutes.Select(r => r.Route).ToArray();
        var routesCounts = topRoutes.Select(r => r.Count).ToArray();

        BusiestRoutesSeries = new ISeries[]
        {
            new RowSeries<int>
            {
                Values = routesCounts,
                DataLabelsPaint = new SolidColorPaint(new SKColor(245, 245, 245)),
                DataLabelsPosition = DataLabelsPosition.Right,
                DataLabelsFormatter = point => point.Model.ToString(),
                Fill = new SolidColorPaint(new SKColor(74, 136, 197))
            }
        };

        BusiestRoutesYAxes = new[]
        {
            new Axis
            {
                Labels = routesLabels,
                LabelsPaint = new SolidColorPaint(new SKColor(200, 200, 200)),
                TextSize = 14
            }
        };
        BusiestRoutesXAxes = new[] { new Axis { IsVisible = false } };


        // 2. Top Airlines (Pie Chart)
        var topAirlines = AirlineQueries.GetTopAirlines(flights);
        var pieSeriesList = new List<ISeries>();

        foreach (var airline in topAirlines)
        {
            pieSeriesList.Add(new PieSeries<int>
            {
                Values = new[] { airline.FlightCount },
                Name = $"{airline.Airline} ({airline.Percentage}%)",
                InnerRadius = 50,
                DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30)),
                DataLabelsPosition = PolarLabelsPosition.Middle,
                DataLabelsFormatter = point => point.Model.ToString()
            });
        }
        TopAirlinesSeries = pieSeriesList.ToArray();

        // 3. Traffic by time of day
        var trafficData = TrafficQueries.GetTrafficByTimeOfDay(flights).ToList();
        var hours = Enumerable.Range(0, 24).ToArray();
        var counts = new int[24];

        foreach (var td in trafficData)
        {
            counts[td.Hour] = td.Count;
        }

        TrafficSeries = new ISeries[]
        {
            new ColumnSeries<int>
            {
                Values = counts,
                Fill = new SolidColorPaint(new SKColor(74, 136, 197))
            }
        };

        TrafficXAxes = new[]
        {
            new Axis
            {
                Labels = hours.Select(h => h.ToString("00")).ToArray(),
                LabelsPaint = new SolidColorPaint(new SKColor(200, 200, 200)),
                MinStep = 2
            }
        };

        TrafficYAxes = new[] { new Axis { IsVisible = false } };
    }
}
