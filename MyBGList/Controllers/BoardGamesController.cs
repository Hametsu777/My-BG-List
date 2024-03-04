﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBGList.Dtos;
using MyBGList.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using MyBGList.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace MyBGList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        private readonly DataContext _context;
        private readonly ILogger<BoardGamesController> _logger;

        public BoardGamesController(DataContext context, ILogger<BoardGamesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Name property is used for Url Generation (based on a specific route) and is not the same as routing.
        // [HttpGet("/someName")] is not the same as [HttpGet( Name = "someName")].
        // Research more about Cache and look up rules, methods, etc, available.
        // Need to add data validation and error handling
        // Look up more about AsQueryable method.
        // Var query line handles the dbSet as an IQueryable object. Did this to be able to chain the extension methods.
        // Var recordCount line line determines the record count. Did this to pull the record count from the database sooner ...
        // so the filter paramet could be taken into account before performing the paging tasks.
        [HttpGet("/GetBoardGames")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        public async Task<RestDto<List<BoardGame>>> GetBoardGames(int pageIndex = 0, int pageSize = 10, string? sortColumn = "Name", string? sortOrder = "ASC", string? filterQuery = null)
        {
            var query = _context.BoardGames.AsQueryable();
            if (!string.IsNullOrEmpty(filterQuery))
            {
                query = query.Where(b => b.Name.Contains(filterQuery));
            };
            var recordCount = await query.CountAsync();
            query = query
                .OrderBy($"{sortColumn} {sortOrder}")
                .Skip(pageIndex * pageSize)
                .Take(pageSize);

            return new RestDto<List<BoardGame>>()
            {
                Data = await query.ToListAsync(),
                PageIndex = pageIndex,
                PageSize = pageSize,
                RecordCount = recordCount,
                Links = new List<LinkDto>
                {
                    new LinkDto(Url.Action(null, "BoardGames", new {pageIndex, pageSize}, Request.Scheme)!, "self", "GET"),
                }


            };
        }

        // Research Update vs Add.
        [HttpPost("/UpdateBoardGame")]
        [ResponseCache(NoStore = true)]
        public async Task<RestDto<BoardGame?>> UpdateBoardGame(BoardGameDto model)
        {
            var boardGame = await _context.BoardGames
                .Where(b => b.Id == model.Id)
                .FirstOrDefaultAsync();
            if (boardGame != null)
            {
                if (!string.IsNullOrEmpty(model.Name))
                {
                    boardGame.Name = model.Name;
                }
                if (model.Year.HasValue && model.Year.Value > 0)
                {
                    boardGame.Year = model.Year.Value;
                }

                boardGame.LastModifiedDate = DateTime.UtcNow;
                _context.BoardGames.Update(boardGame);
                await _context.SaveChangesAsync();
            }

            return new RestDto<BoardGame?>()
            {
                Data = boardGame,
                Links = new List<LinkDto>
                {
                    new LinkDto(Url.Action(null, "BoardGames", model, Request.Scheme)!, "self", "POST"),
                }
            };
        }

    }
}

//Data = new List<BoardGame>()
//{
//    new BoardGame() {
//        Id = 1,
//        Name = "Axis & Aliies",
//        Year = 1981,
//        //MinPlayers = 2,
//        //MaxPlayers = 5,
//    },
//    new BoardGame()
//    {
//        Id = 2,
//        Name = "Citadels",
//        Year = 2000,
//        //MinPlayers = 2,
//        //MaxPlayers = 8,
//    },
//    new BoardGame()
//    {
//        Id = 3,
//        Name = "Terraforming Mars",
//        Year = 2016,
//        //MinPlayers = 1,
//        //MaxPlayers = 5,

//    }