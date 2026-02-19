using Shkodran_Hasani_Zgjedhjet_API.Models.DTOs;

namespace Shkodran_Hasani_Zgjedhjet_API.Services
{
    public interface ICsvImportService
    {
        Task<CsvImportResponse> ImportCsvAsync(IFormFile file);
    }
}
