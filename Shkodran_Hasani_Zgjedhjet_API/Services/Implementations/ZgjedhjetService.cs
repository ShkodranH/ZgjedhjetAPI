using Microsoft.EntityFrameworkCore;
using Shkodran_Hasani_Zgjedhjet_API.Data;
using Shkodran_Hasani_Zgjedhjet_API.Enums;
using Shkodran_Hasani_Zgjedhjet_API.Models.DTOs;
using Shkodran_Hasani_Zgjedhjet_API.Models.Entities;
using System.Globalization;

namespace Shkodran_Hasani_Zgjedhjet_API.Services.Implementations
{
    public class ZgjedhjetService : IZgjedhjetService
    {
        private readonly LifeDbContext _context;

        public ZgjedhjetService(LifeDbContext context)
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

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);

            string? headerLine = await reader.ReadLineAsync();
            if (headerLine == null)
            {
                response.Success = false;
                response.Message = "Empty CSV file";
                return response;
            }

            int imported = 0;
            var errors = new List<string>();

            while (!reader.EndOfStream)
            {
                string? line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var columns = line.Split(',');

                try
                {
                    if (columns.Length != 6)
                        throw new Exception("Incorrect number of columns");

                    var zgjedhje = new Zgjedhjet
                    {
                        Kategoria = Enum.Parse<Kategoria>(columns[0]),
                        Komuna = Enum.Parse<Komuna>(columns[1]),
                        QendraVotimit = columns[2].Trim(),
                        VendVotimi = columns[3].Trim(),
                        Partia = Enum.Parse<Partia>(columns[4]),
                        Vota = int.Parse(columns[5], CultureInfo.InvariantCulture)
                    };

                    if (string.IsNullOrEmpty(zgjedhje.QendraVotimit) || string.IsNullOrEmpty(zgjedhje.VendVotimi))
                        throw new Exception("QendraVotimit or VendVotimi is empty");

                    _context.Zgjedhjet.Add(zgjedhje);
                    imported++;
                }
                catch (Exception ex)
                {
                    errors.Add($"Line {imported + errors.Count + 2}: {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();

            response.Success = errors.Count == 0;
            response.RecordsImported = imported;
            response.Errors = errors;
            response.Message = response.Success
                ? $"Successfully imported {imported} records"
                : $"Imported {imported} records with {errors.Count} errors";

            return response;
        }

        public async Task<ZgjedhjetAggregatedResponse> GetAggregatedDataAsync(
            Kategoria? kategoria = null,
            Komuna? komuna = null,
            string? qendra_e_votimit = null,
            string? vendvotimi = null,
            Partia? partia = null)
        {
            var query = _context.Zgjedhjet.AsQueryable();

            if (kategoria.HasValue && kategoria.Value != Kategoria.TeGjitha)
                query = query.Where(x => x.Kategoria == kategoria.Value);

            if (komuna.HasValue && komuna.Value != Komuna.TeGjitha)
                query = query.Where(x => x.Komuna == komuna.Value);

            if (!string.IsNullOrWhiteSpace(qendra_e_votimit))
                query = query.Where(x => x.QendraVotimit == qendra_e_votimit);

            if (!string.IsNullOrWhiteSpace(vendvotimi))
                query = query.Where(x => x.VendVotimi == vendvotimi);

            if (partia.HasValue && partia.Value != Partia.TeGjitha)
                query = query.Where(x => x.Partia == partia.Value);

            var aggregated = await query
                .GroupBy(x => x.Partia)
                .Select(g => new PartiaVotesResponse
                {
                    Partia = g.Key.ToString(),
                    TotalVota = g.Sum(x => x.Vota)
                })
                .ToListAsync();

            return new ZgjedhjetAggregatedResponse { Results = aggregated };
        }
    }
}
