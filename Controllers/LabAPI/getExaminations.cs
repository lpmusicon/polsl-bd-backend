using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
//niegotowe
namespace bd_backend.Controllers
{
    public class examinationGetter
    {
        public string Status { get; set; }
    };

    [ApiController]
    [Route("api/lab/getexaminations")]
    public class getExaminationsController : ControllerBase
    {
        public string Get()
        {

            var result = (from x in new DatabaseContext().LaboratoryExaminations select new userGetter { UserId = x.UserId, Login = x.Login, Role = x.Role, DisabledTo = x.DisabledTo }).ToList();
            return JsonSerializer.Serialize<System.Collections.Generic.List<bd_backend.Controllers.examinationGetter>>(result);
        }
    }
}

