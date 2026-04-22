using System.Collections.Generic;
using AOOP3.Models;

namespace AOOP3.Services
{
    public interface IFlightService
    {
        IEnumerable<Airport> GetAllAirports();
        IEnumerable<object> GetAllFlights();
    }
}