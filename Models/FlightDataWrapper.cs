using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AOOP3.Models;

public class FlightDataWrapper 
{
    [JsonPropertyName("airports")]
    public List<Airport> Airports { get; set; } = new();
    
    [JsonPropertyName("flights")]
    public List<Flight> Flights { get; set; } = new();
}