using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AOOP3.Models;

namespace AOOP3.Services;

public class ExportService : IExportService
{
    public async Task ExportFlightsCsvAsync(IEnumerable<Flight> flights, string filePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("FlightNumber,Airline,DepartureAirport,ArrivalAirport,ScheduledDeparture,ScheduledArrival,Status,AircraftType");

        foreach (var f in flights)
        {
            var line = $"{Escape(f.FlightNumber)},{Escape(f.AirlineName)},{Escape(f.DepartureAirport)},{Escape(f.ArrivalAirport)},{f.ScheduledDeparture:O},{f.ScheduledArrival:O},{Escape(f.Status.ToString())},{Escape(f.AircraftType)}";
            sb.AppendLine(line);
        }

        await File.WriteAllTextAsync(filePath, sb.ToString());
    }

    public async Task ExportAnalyticsTxtAsync(string content, string filePath)
    {
        await File.WriteAllTextAsync(filePath, content);
    }

    private string Escape(string? value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        if (value.Contains(",") || value.Contains("\""))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
        return value;
    }
}
