using System.Collections.Generic;
using System.Linq;
using AOOP3.Models;

namespace AOOP3.Queries;

public static class TrafficQueries
{
    public static IEnumerable<(int Hour, int Count)> GetTrafficByTimeOfDay(IEnumerable<Flight> flights)
    {
        return flights
            .GroupBy(f => f.ScheduledDeparture.Hour)
            .Select(g => (Hour: g.Key, Count: g.Count()))
            .OrderBy(x => x.Hour);
    }
}
