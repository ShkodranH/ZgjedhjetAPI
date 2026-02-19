using Shkodran_Hasani_Zgjedhjet_API.Enums;
using Shkodran_Hasani_Zgjedhjet_API.Models.DTOs;

namespace Shkodran_Hasani_Zgjedhjet_API.Services
{
    public interface IZgjedhjetService
    {
        Task<CsvImportResponse> ImportCsvAsync(IFormFile file);
        Task<ZgjedhjetAggregatedResponse> GetAggregatedDataAsync(
            Kategoria? kategoria = null,
            Komuna? komuna = null,
            string? qendra_e_votimit = null,
            string? vendvotimi = null,
            Partia? partia = null);
    }
}
