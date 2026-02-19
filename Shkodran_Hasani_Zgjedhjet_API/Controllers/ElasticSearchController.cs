using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shkodran_Hasani_Zgjedhjet_API.Data;
using Shkodran_Hasani_Zgjedhjet_API.Models.Elastic;
using Shkodran_Hasani_Zgjedhjet_API.Models.Entities;
using Shkodran_Hasani_Zgjedhjet_API.Services;
using Shkodran_Hasani_Zgjedhjet_API.Services.Implementations;

namespace Shkodran_Hasani_Zgjedhjet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElasticSearchController : ControllerBase
    {
        private readonly LifeDbContext _dbContext;
        private readonly IElasticService<ZgjedhjeElasticDocument> _elasticService;
        private readonly IRedisService _redisService;

        public ElasticSearchController(
            LifeDbContext dbContext,
            IElasticService<ZgjedhjeElasticDocument> elasticService,
            IRedisService redisService)
        {
            _dbContext = dbContext;
            _elasticService = elasticService;
            _redisService = redisService;
        }


        [HttpPost("import")]
        public async Task<IActionResult> ImportDataToElasticsearch()
        {
            var zgjedhjet = await _dbContext.Set<Zgjedhjet>().ToListAsync();

            var documents = zgjedhjet.Select(z => new ZgjedhjeElasticDocument
            {
                Id = z.Id,
                Komuna = z.Komuna.ToString(),
                Partia = z.Partia.ToString(),
                Kategoria = z.Kategoria.ToString(),
                Vota = z.Vota
            });

            await _elasticService.IndexManyAsync(documents);

            return Ok("Të dhënat janë importuar me sukses në Elasticsearch.");
        }


        [HttpGet("search")]
        public async Task<IActionResult> SearchZgjedhjet(
            [FromForm] string? komuna,
            [FromForm] string? partia,
            [FromForm] string? kategoria)
        {
            var results = await _elasticService.SearchAsync(komuna, partia, kategoria);
            return Ok(results);
        }


        [HttpGet("suggest-komuna")]
        public async Task<IActionResult> SuggestKomuna([FromQuery] string query, [FromQuery] int top = 5)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Ok(new List<string>());

            var suggestions = await _elasticService.SuggestKomunaAsync(query, top);

            // Regjistro statistikat në Redis
            if (suggestions.Any())
            {
                await _redisService.IncrementSuggestionStats(suggestions);
            }

            return Ok(suggestions);
        }
    }
}
