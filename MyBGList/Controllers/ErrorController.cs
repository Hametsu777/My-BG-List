using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyBGList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [HttpGet("/error")]
        public IActionResult Error()
        {
            return Problem();
        }

        [HttpGet("/error/test")]
        public IActionResult Test()
        {
            throw new Exception("test");
        }
    }
}
