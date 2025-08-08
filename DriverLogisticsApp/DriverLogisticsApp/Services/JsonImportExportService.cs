using DriverLogisticsApp.Models;
using System.Text.Json;
using CommunityToolkit.Maui.Storage;
using System.Text;

namespace DriverLogisticsApp.Services
{
    public class JsonImportExportService : IJsonImportExportService
    {
        public async Task ExportDataAsync(ExportData data, string initialFileName)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

            await FileSaver.Default.SaveAsync(initialFileName, stream, CancellationToken.None);
        }

        public async Task<ExportData?> ImportDataAsync()
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select a JSON backup file",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".json" } },
                    { DevicePlatform.iOS, new[] { "public.json" } },
                    { DevicePlatform.Android, new[] { "application/json" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.json" } }
                })
            });

            if (result == null)
                return null;

            using var stream = await result.OpenReadAsync();
            return await JsonSerializer.DeserializeAsync<ExportData>(stream);
        }
    }
}
