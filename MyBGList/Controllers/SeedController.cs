using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBGList.Data;
using MyBGList.Models;
using MyBGList.Models.Csv;
using System.Globalization;

namespace MyBGList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<SeedController> _logger;

        public SeedController(DataContext context, IWebHostEnvironment env, ILogger<SeedController> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        // Need to research more about CSV MEthods, etc.
        [HttpPut(Name = "Seed")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Put()
        {
            // SETUP
            // Instantiating a CsvConfiguration object to set up some csv properties.
            // Creating a StreamReader pointing to the Csv file. Creating an instance of the CsvReader class
            // which will perform the CSV parsing and map each record to a BGGRecord object.
            // StreamReader and CsvReader objects have been declared with a using declaration. This ensures
            // the correct use of IDisposable and IAsyncDisposable objects by disposing of them at the end of the scope.
            var config = new CsvConfiguration(CultureInfo.GetCultureInfo("pt-BR"))
            {
                HasHeaderRecord = true,
                Delimiter = ";",
            };
            using var reader = new StreamReader(
                System.IO.Path.Combine(_env.ContentRootPath, "DataSets/bgg_dataset.csv"));
            using var csv = new CsvReader(reader, config);
            var existingBoardGames = await _context.BoardGames.ToDictionaryAsync(bg => bg.Id);
            var existingDomains = await _context.Domains.ToDictionaryAsync(d => d.Name);
            var existingMechanics = await _context.Mechanics.ToDictionaryAsync(m => m.Name);
            var now = DateTime.UtcNow;

            // EXECUTE
            var records = csv.GetRecords<BggRecord>();
            var skippedRows = 0;
            foreach (var record in records)
            {
                if (!record.ID.HasValue || string.IsNullOrEmpty(record.Name) || existingBoardGames.ContainsKey(record.ID.Value))
                {
                    skippedRows++;
                    continue;
                }

                var boardgame = new BoardGame()
                {

                };
            }


        }
    }
}
