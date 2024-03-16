using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyBGList.Data;
using MyBGList.Dtos;
using MyBGList.Models;
using System.Linq.Dynamic.Core;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MyBGList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MechanicsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ILogger<MechanicsController> _logger;

        public MechanicsController(DataContext context, ILogger<MechanicsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("/GetMechanics")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        public async Task<RestDto<List<Mechanic>>> GetMechanics([FromQuery] RequestDto<MechanicDto> input)
        {
            var query = _context.Mechanics.AsQueryable();
            if (!string.IsNullOrEmpty(input.FilterQuery))
            {
                query.Where(b => b.Name.Contains(input.FilterQuery));
            }
            var recordCount = await query.CountAsync();
            query = query.OrderBy($"{input.SortColumn} {input.SortOrder}")
                .Skip(input.PageIndex * input.PageSize)
                .Take(input.PageSize);

            return new RestDto<List<Mechanic>>()
            {
                Data = await query.ToListAsync(),
                PageIndex = input.PageIndex,
                PageSize = input.PageSize,
                RecordCount = recordCount,
                Links = new List<LinkDto>
                {
                    new LinkDto(Url.Action(null,"Mechanics", new {input.PageIndex, input.PageSize}, Request.Scheme)!, "self", "GET")
                }
            };
        }

        [HttpPost("UpdateMechanic")]
        [ResponseCache(NoStore = true)]
        public async Task<RestDto<Mechanic?>> UpdateMechanic(MechanicDto model)
        {
            var mechanic = await _context.Mechanics
                .Where(b => b.Id == model.Id)
                .FirstOrDefaultAsync();
            if (mechanic != null)
            {
                if (!string.IsNullOrEmpty(model.Name))
                {
                    mechanic.Name = model.Name;
                }
                mechanic.LastModifiedDate = DateTime.UtcNow;
                _context.Mechanics.Update(mechanic);
                await _context.SaveChangesAsync();
            }

            return new RestDto<Mechanic?>()
            {
                Data = mechanic,
                Links = new List<LinkDto>
                {
                    new LinkDto(Url.Action(null,"Mechanics", model, Request.Scheme)!, "self", "POST")
                }
            };
        }

        [HttpDelete("/DeleteMechanic")]
        [ResponseCache(NoStore = true)]
        public async Task<RestDto<Mechanic?>> DeleteMechanic(int id)
        {
            var mechanic = await _context.Mechanics
                .Where(b => b.Id == id)
                .FirstOrDefaultAsync();
            if (mechanic != null)
            {
                _context.Mechanics.Remove(mechanic);
                await _context.SaveChangesAsync();
            }

            return new RestDto<Mechanic?>()
            {
                Data = mechanic,
                Links = new List<LinkDto>
                {
                    new LinkDto(Url.Action(null,"Mechanics", id, Request.Scheme)!, "self", "DELETE")
                }
            };
        }
    }
}
