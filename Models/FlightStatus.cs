using System.Text.Json.Serialization;

namespace AOOP3.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FlightStatus
{
    Scheduled,
    Landed,
    Delayed,
    Cancelled
}