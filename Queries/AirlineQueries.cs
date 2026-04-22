using System;
using System.Collections.Generic;
using System.Linq;
using AOOP3.Models;

namespace AOOP3.Queries;

public static class AirlineQueries
{
    public static IEnumerable<(string Airline, int FlightCount, double Percentage)> GetTopAirlines(IEnumerable<Flight> flights, int topCount = 3)
    {
        var totalFlights = flights.Count();
        if (totalFlights == 0) return Enumerable.Empty<(string, int, double)>();

        var grouped = flights
            .GroupBy(f => f.AirlineName)
            .Select(g => new { Airline = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToList();

        var top = grouped.Take(topCount).Select(x => (x.Airline, x.Count, Math.Round((double)x.Count / totalFlights * 100, 1))).ToList();

        var othersCount = grouped.Skip(topCount).Sum(x => x.Count);
        if (othersCount > 0)
        {
            top.Add(("Others", othersCount, Math.Round((double)othersCount / totalFlights * 100, 1)));
        }

        return top;
    }
}
