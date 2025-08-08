using DriverLogisticsApp.Models;

namespace DriverLogisticsApp.Services
{
    public interface IJsonImportExportService
    {
        Task ExportDataAsync(ExportData data, string initialFileName);
        Task<ExportData?> ImportDataAsync();
    }
}
