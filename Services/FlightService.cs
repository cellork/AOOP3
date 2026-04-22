using System.Collections.Generic;
using AOOP3.Models;

namespace AOOP3.Services
{
    public class FlightService : IFlightService
    {
        private readonly IDataRepository _repository;

        public FlightService(IDataRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Airport> GetAllAirports() => _repository.GetAirports();

        public IEnumerable<Flight> GetAllFlights() => _repository.GetFlights();
    }
}
