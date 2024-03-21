using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBGList.Attributes;
using MyBGList.Data;
using MyBGList.Dtos;
using MyBGList.Models;
using System.Linq.Dynamic.Core;

namespace MyBGList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DomainsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ILogger<DomainsController> _logger;

        public DomainsController(DataContext context, ILogger<DomainsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("/GetDomains")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        [ManualValidationFilter]
        public async Task<ActionResult<RestDto<List<Domain>>>> GetDomains([FromQuery] RequestDto<DomainDto> input)
        {
            if (!ModelState.IsValid)
            {
                var details = new ValidationProblemDetails(ModelState);
                details.Extensions["traceId"] = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier;

                if (ModelState.Keys.Any(k => k == "PageSize"))
                {
                    details.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.2";
                    details.Status = StatusCodes.Status501NotImplemented;

                    return new ObjectResult(details)
                    {
                        StatusCode = StatusCodes.Status501NotImplemented,
                    };
                }
                else
                {
                    details.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                    details.Status = StatusCodes.Status400BadRequest;
                    return new BadRequestObjectResult(details);
                }
            }

            var query = _context.Domains.AsQueryable();
            if (!string.IsNullOrEmpty(input.FilterQuery))
            {
                query = query.Where(b => b.Name.Contains(input.FilterQuery));
            }
            var recordCount = await query.CountAsync();
            query = query
                .OrderBy($"{input.SortColumn} {input.SortOrder}")
                .Skip(input.PageIndex * input.PageSize)
                .Take(input.PageSize);

            return new RestDto<List<Domain>>()
            {
                Data = await query.ToListAsync(),
                PageIndex = input.PageIndex,
                PageSize = input.PageSize,
                RecordCount = recordCount,
                Links = new List<LinkDto>
                {
                    new LinkDto(Url.Action(null, "Domains", new {input.PageIndex, input.PageSize}, Request.Scheme)!, "self", "GET"),
                }
            };
        }

        [HttpPost("/UpdateDomain")]
        [ResponseCache(NoStore = true)]
        public async Task<RestDto<Domain?>> UpdateDomain(DomainDto model)
        {
            var domain = await _context.Domains
                .Where(b => b.Id == model.Id)
                .FirstOrDefaultAsync();
            if (domain != null)
            {
                if (!string.IsNullOrEmpty(model.Name))
                {
                    domain.Name = model.Name;
                }

                domain.LastModifiedDate = DateTime.UtcNow;
                _context.Domains.Update(domain);
                await _context.SaveChangesAsync();
            }

            return new RestDto<Domain?>()
            {
                Data = domain,
                Links = new List<LinkDto>
                {
                    new LinkDto(Url.Action(null, "Domains", model, Request.Scheme)!, "self", "POST"),
                }
            };
        }

        [HttpDelete("/DeleteDomain")]
        [ResponseCache(NoStore = true)]
        public async Task<RestDto<Domain?>> DeleteDomain(int id)
        {
            var domain = await _context.Domains
                .Where(b => b.Id == id)
                .FirstOrDefaultAsync();
            if (domain != null)
            {
                _context.Domains.Remove(domain);
                await _context.SaveChangesAsync();
            }

            return new RestDto<Domain?>()
            {
                Data = domain,
                Links = new List<LinkDto>
                {
                    new LinkDto(Url.Action(null, "Domains", id, Request.Scheme)!, "Self", "DELETE")
                }
            };
        }


    }
}
