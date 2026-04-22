using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using AOOP3.Models;
using AOOP3.Services;
using AOOP3.ViewModels;

namespace AOOP3.Tests
{
    public class ViewModelTests
    {
        private List<Flight> GetMockFlights()
        {
            return new List<Flight>
            {
                new Flight { FlightNumber = "FL1", AirlineName = "SAS", DepartureAirport = "CPH", ArrivalAirport = "AAL", Status = FlightStatus.Scheduled },
                new Flight { FlightNumber = "FL2", AirlineName = "SAS", DepartureAirport = "SGD", ArrivalAirport = "CPH", Status = FlightStatus.Delayed },
                new Flight { FlightNumber = "FL3", AirlineName = "Norwegian", DepartureAirport = "CPH", ArrivalAirport = "OSL", Status = FlightStatus.Cancelled }
            };
        }

        private List<Airport> GetMockAirports()
        {
            return new List<Airport>
            {
                new Airport { IataCode = "CPH", Name = "Copenhagen Airport" },
                new Airport { IataCode = "SGD", Name = "Sonderborg Airport" },
                new Airport { IataCode = "AAL", Name = "Aalborg Airport" },
                new Airport { IataCode = "OSL", Name = "Oslo Airport" }
            };
        }

        [Fact]
        public void FlightInfoViewModel_FiltersFlights_ByStatus()
        {
            // Arrange
            var mockService = new Mock<IFlightService>();
            mockService.Setup(s => s.GetAllFlights()).Returns(GetMockFlights());
            mockService.Setup(s => s.GetAllAirports()).Returns(GetMockAirports());

            var viewModel = new FlightInfoViewModel(mockService.Object);

            // Act: Select CPH and filter by "Delayed"
            // Wait, FL2 departs from SGD. FL1 and FL3 depart from CPH.
            viewModel.SelectedAirport = viewModel.FilteredAirports.First(a => a.IataCode == "CPH");
            viewModel.SelectedStatus = "Cancelled";

            // Assert
            // Expecting 1 cancelled flight departing from CPH (FL3)
            Assert.Single(viewModel.FilteredFlights);
            Assert.Equal("FL3", viewModel.FilteredFlights.First().FlightNumber);
        }

        [Fact]
        public void AnalyticsViewModel_RouteQueries_AreCorrect()
        {
            // Arrange
            var mockService = new Mock<IFlightService>();
            mockService.Setup(s => s.GetAllFlights()).Returns(GetMockFlights());
            mockService.Setup(s => s.GetAllAirports()).Returns(GetMockAirports());

            var viewModel = new AnalyticsViewModel(mockService.Object);

            // Accessing the values from Series
            // BusiestRoutesYAxes contains the labels
            var yAxes = viewModel.BusiestRoutesYAxes.FirstOrDefault();
            Assert.NotNull(yAxes);
            Assert.NotNull(yAxes.Labels);
            Assert.Equal(3, yAxes.Labels.Count);
        }

        [Fact]
        public void AnalyticsViewModel_AirlineQueries_AreCorrect()
        {
            // Arrange
            var mockService = new Mock<IFlightService>();
            mockService.Setup(s => s.GetAllFlights()).Returns(GetMockFlights());
            mockService.Setup(s => s.GetAllAirports()).Returns(GetMockAirports());

            var viewModel = new AnalyticsViewModel(mockService.Object);

            // Top Airlines should show SAS with 2 flights, Norwegian with 1
            Assert.Equal(2, viewModel.TopAirlinesSeries.Length);
        }
    }
}
