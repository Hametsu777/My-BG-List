using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBGList.Dtos;
using MyBGList.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using MyBGList.Dtos.v2;
using Asp.Versioning;

namespace MyBGList.Controllers.v2
{
    [Route("/v{version:ApiVersion}/api/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class BoardGamesController : ControllerBase
    {
        // Trying to make a List but may have to change back to an array.
        //private RestDto<List<BoardGame>> lo = new RestDto<List<BoardGame>>()
        //{
        //    Data = new List<BoardGame>()
        //    {
        //        new BoardGame()
        //        {
        //        Id = 1,
        //        Name = "Axis & Allies",
        //        Year = 1981,
        //        },
        //        new BoardGame()
        //        {
        //            Id = 2,
        //            Name = "Citadels",
        //            Year = 2000,
        //        },
        //        new BoardGame()
        //        {
        //            Id = 3,
        //            Name = "Terraforming Mars",
        //            Year = 2016,
        //        }

        //    },
        //    Links = new List<LinkDto>
        //    {
        //        new LinkDto(Url.Action(null, "BoardGames", null, Request.Scheme)!, "self", "GET")
        //    }

        //};

        private readonly ILogger<BoardGamesController> _logger;

        public BoardGamesController(ILogger<BoardGamesController> logger)
        {
            _logger = logger;
        }

        // Name property is used for Url Generation (based on a specific route) and is not the same as routing.
        // [HttpGet("/someName")] is not the same as [HttpGet( Name = "someName")].
        // Research more about Cache and look up rules, methods, etc, available.
        [HttpGet("/GetBoardGames")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        public RestDto<List<BoardGame>> GetBoardGames()
        {
            return new RestDto<List<BoardGame>>()
            {
                Items = new List<BoardGame>()
                {
                    new BoardGame() {
                        Id = 1,
                        Name = "Axis & Aliies",
                        Year = 1981,
                        //MinPlayers = 2,
                        //MaxPlayers = 5,
                    },
                    new BoardGame()
                    {
                        Id = 2,
                        Name = "Citadels",
                        Year = 2000,
                        //MinPlayers = 2,
                        //MaxPlayers = 8,
                    },
                    new BoardGame()
                    {
                        Id = 3,
                        Name = "Terraforming Mars",
                        Year = 2016,
                        //MinPlayers = 1,
                        //MaxPlayers = 5,

                    }
                },
                Links = new List<Dtos.v1.LinkDto>
                {
                    new Dtos.v1.LinkDto(Url.Action(null, "BoardGames", null, Request.Scheme)!, "self", "GET"),
                }

            };
        }
    }
}
