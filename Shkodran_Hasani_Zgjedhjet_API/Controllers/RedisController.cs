using Microsoft.AspNetCore.Mvc;
using Shkodran_Hasani_Zgjedhjet_API.Services;

namespace Shkodran_Hasani_Zgjedhjet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly IRedisService _redisService;

        public RedisController(IRedisService redisService)
        {
            _redisService = redisService;
        }


        [HttpPost("increment-suggestions")]
        public async Task<IActionResult> IncrementSuggestions([FromBody] List<string> suggestions)
        {
            if (suggestions == null || suggestions.Count == 0)
                return BadRequest("Lista e sugjerimeve është bosh.");

            await _redisService.IncrementSuggestionStats(suggestions);
            return Ok("Sugjerimet u regjistruan në Redis me sukses.");
        }


        [HttpGet("top-suggestions")]
        public async Task<IActionResult> GetTopSuggestions([FromQuery] int top = 10)
        {
            var stats = await _redisService.GetTopSuggestions(top);
            var result = stats.Select(s => new
            {
                komuna = s.Komuna,
                nrISugjerimeve = s.Count
            });

            return Ok(result);
        }
    }
}
