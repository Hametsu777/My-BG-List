﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyBGList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [HttpGet("/errorx")]
        public IActionResult Error()
        {
            return Problem();
        }

        [HttpGet("/errorx/test")]
        public IActionResult Test()
        {
            throw new Exception("test");
        }
    }
}
