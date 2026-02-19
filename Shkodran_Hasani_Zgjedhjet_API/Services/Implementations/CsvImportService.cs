using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Shkodran_Hasani_Zgjedhjet_API.Data;
using Shkodran_Hasani_Zgjedhjet_API.Enums;
using Shkodran_Hasani_Zgjedhjet_API.Models.DTOs;
using Shkodran_Hasani_Zgjedhjet_API.Models.Entities;
using System.Globalization;

namespace Shkodran_Hasani_Zgjedhjet_API.Services.Implementations
{
    public class CsvImportService : ICsvImportService
    {
        private readonly LifeDbContext _context;
        private const int BatchSize = 500;

        public CsvImportService(LifeDbContext context)
        {
            _context = context;
        }

        public async Task<CsvImportResponse> ImportCsvAsync(IFormFile file)
        {
            var response = new CsvImportResponse();

            if (file == null || file.Length == 0)
            {
                response.Success = false;
                response.Message = "No file provided";
                return response;
            }

            using var reader = new StreamReader(file.OpenReadStream());
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                BadDataFound = null
            };

            using var csv = new CsvReader(reader, config);
            var records = csv.GetRecords<ZgjedhjetCsv>();

            var entities = new List<Zgjedhjet>();
            int lineNumber = 1;

            foreach (var record in records)
            {
                lineNumber++;

                try
                {
                    if (!Enum.TryParse<Kategoria>(record.Kategoria, true, out var kategoria))
                        throw new Exception($"Invalid Kategoria: '{record.Kategoria}'");

                    if (!Enum.TryParse<Komuna>(record.Komuna, true, out var komuna))
                        throw new Exception($"Invalid Komuna: '{record.Komuna}'");

                    if (!Enum.TryParse<Partia>(record.Partia, true, out var partia))
                        throw new Exception($"Invalid Partia: '{record.Partia}'");

                    if (string.IsNullOrWhiteSpace(record.Qendra_e_Votimit) || string.IsNullOrWhiteSpace(record.VendVotimi))
                        throw new Exception("Qendra_e_Votimit or VendVotimi is empty");

                    entities.Add(new Zgjedhjet
                    {
                        Kategoria = kategoria,
                        Komuna = komuna,
                        QendraVotimit = record.Qendra_e_Votimit,
                        VendVotimi = record.VendVotimi,
                        Partia = partia,
                        Vota = record.Vota
                    });

                    response.RecordsImported++;
                }
                catch (Exception ex)
                {
                    response.Errors.Add($"Line {lineNumber}: {ex.Message}");
                }

                if (entities.Count >= BatchSize)
                {
                    await _context.Zgjedhjet.AddRangeAsync(entities);
                    await _context.SaveChangesAsync();
                    entities.Clear();
                }
            }

            if (entities.Count > 0)
            {
                await _context.Zgjedhjet.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
            }

            response.Success = response.Errors.Count == 0;
            response.Message = response.Success
                ? $"Successfully imported {response.RecordsImported} records"
                : $"Imported {response.RecordsImported} records with {response.Errors.Count} errors";

            return response;
        }
    }
}
