using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using BackendProject.Models;

namespace BackendProject.Controllers
{
    [ApiController]
    [Route("reception/tea")]
    public class TeaTime : ControllerBase
    {
        public IActionResult Get()
        {
            return StatusCode(418, "{ \"Reason\": \"Tea Time\" }");
        }
    }
}
