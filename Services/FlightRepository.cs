using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using AOOP3.Models;

namespace AOOP3.Services
{
    public class FlightRepository : IDataRepository
    {
        private FlightDataWrapper _dataWrapper;

        public FlightRepository()
        {
            _dataWrapper = new FlightDataWrapper();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Flights.json");
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new JsonStringEnumConverter() }
                    };
                    _dataWrapper = JsonSerializer.Deserialize<FlightDataWrapper>(json, options) ?? new FlightDataWrapper();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        public IEnumerable<Airport> GetAirports() => _dataWrapper.Airports;
        public IEnumerable<Flight> GetFlights() => _dataWrapper.Flights;
    }
}
