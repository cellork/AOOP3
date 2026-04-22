using System.Collections.Generic;
using AOOP3.Models;

namespace AOOP3.Services
{
    public interface IDataRepository
    {
        IEnumerable<Airport> GetAirports();
        IEnumerable<Flight> GetFlights();
    }
}
