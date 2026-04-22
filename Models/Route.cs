using System.Collections.Generic;

namespace AOOP3.Models;

public class Route
{
    public Airport DepartureAirport { get; }
    public Airport ArrivalAirport { get; }
    
    public List<Flight> Flights { get; } = new();

    public Route(Airport departureAirport, Airport arrivalAirport)
    {
        DepartureAirport = departureAirport;
        ArrivalAirport = arrivalAirport;
    }

    public string RouteDisplayName => $"{DepartureAirport.IataCode} ➔ {ArrivalAirport.IataCode}";
}