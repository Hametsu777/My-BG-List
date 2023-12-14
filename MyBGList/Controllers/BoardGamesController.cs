using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBGList.Models;

namespace MyBGList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardGamesController : ControllerBase
    {
        private static List<BoardGame> boardGames = new List<BoardGame>
        {
            new BoardGame()
            {
                Id = 1,
                Name = "Axis & Aliies",
                Year = 1981,
                MinPlayers = 2,
                MaxPlayers = 5,
            },
            new BoardGame()
            {
                Id = 2,
                Name = "Citadels",
                Year = 2000,
                MinPlayers = 2,
                MaxPlayers = 8,
            },
            new BoardGame()
            {
                Id = 3,
                Name = "Terraforming Mars",
                Year = 2016,
                MinPlayers = 1,
                MaxPlayers = 5,
            }
    };
        private readonly ILogger<BoardGamesController> _logger;

        public BoardGamesController(ILogger<BoardGamesController> logger)
        {
            _logger = logger;
        }

        // Name property is used for Url Generation (based on a specific route) and is not the same as routing.
        // [HttpGet("/someName")] is not the same as [HttpGet( Name = "someName")].
        [HttpGet("/GetBoardGames")]
        public IEnumerable<BoardGame> GetBoardGames()
        {
            return boardGames;
        }
    }
}
