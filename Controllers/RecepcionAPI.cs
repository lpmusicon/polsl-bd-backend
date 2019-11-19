using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendProject.Controllers
{
    [ApiController]
    [Route("reception/tea")]
    [Authorize(Roles="RECP, DOCT")]
    public class TeaTime : ControllerBase
    {
        public IActionResult Get()
        {
            return StatusCode(418, "{ \"Reason\": \"Tea Time\" }");
        }
    }
}
