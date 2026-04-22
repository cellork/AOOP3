using System.Collections.Generic;
using System.Threading.Tasks;
using AOOP3.Models;

namespace AOOP3.Services;

public interface IExportService
{
    Task ExportFlightsCsvAsync(IEnumerable<Flight> flights, string filePath);
    Task ExportAnalyticsTxtAsync(string content, string filePath);
}
