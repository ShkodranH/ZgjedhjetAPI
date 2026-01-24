using Microsoft.AspNetCore.Mvc;
using Shkodran_Hasani_Zgjedhjet_API.Enums;
using Shkodran_Hasani_Zgjedhjet_API.Models.DTOs;
using Shkodran_Hasani_Zgjedhjet_API.Services;

namespace ZgjedhjetApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZgjedhjetController : ControllerBase
    {
        // YOUR CODE HERE
        // IT IS UP TO YOU TO DECIDE IF YOU WILL USE DB CONTEXT HERE OR THROUGH SERVICES/REPOSITORIES
        private readonly ICsvImportService _csvService;
        private readonly IZgjedhjetService _service;
        private readonly ILogger<ZgjedhjetController> _logger;

        public ZgjedhjetController(IZgjedhjetService service, ICsvImportService csvService, ILogger<ZgjedhjetController> logger)
        {
            _csvService = csvService;
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// POST endpoint to import CSV file
        /// </summary>
        [HttpPost("import")]
        public async Task<ActionResult<CsvImportResponse>> MigrateData(IFormFile file)
        {
            // YOUR CODE HERE
            if (file == null || file.Length == 0)
                return BadRequest(new CsvImportResponse
                {
                    Success = false,
                    Message = "No file provided"
                });

            var response = await _csvService.ImportCsvAsync(file); // <-- use _csvService here
            return response.Success ? Ok(response) : BadRequest(response);
            //var response = new CsvImportResponse();
            //return Ok(response);
        }

        /// <summary>
        /// GET endpoint to retrieve and filter electoral data
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ZgjedhjetAggregatedResponse>> GetZgjedhjet(
            [FromQuery] Kategoria? kategoria = null,
            [FromQuery] Komuna? komuna = null,
            [FromQuery] string? qendra_e_votimit = null,
            [FromQuery] string? vendvotimi = null,
            [FromQuery] Partia? partia = null)
        {
            // YOUR CODE HERE
            var response = await _service.GetAggregatedDataAsync(kategoria, komuna, qendra_e_votimit, vendvotimi, partia);
            return Ok(response);
            //var response = new ZgjedhjetAggregatedResponse();
            //return Ok(response);
        }
    }
}
