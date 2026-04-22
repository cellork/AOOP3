using System.Collections.Generic;
using System.Linq;
using AOOP3.Models;

namespace AOOP3.Queries;

public static class RouteQueries
{
    public static IEnumerable<(string Route, int Count)> GetBusiestRoutes(IEnumerable<Flight> flights, int topCount = 5)
    {
        return flights
            .GroupBy(f => $"{f.DepartureAirport}→{f.ArrivalAirport}")
            .Select(g => (Route: g.Key, Count: g.Count()))
            .OrderByDescending(r => r.Count)
            .Take(topCount);
    }
}
