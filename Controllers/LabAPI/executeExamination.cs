using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace bd_backend.Controllers
{
    public class examination
    {
        public int LaboratoryExaminationsId { get; set; }
        public string Result { get; set; }
        public int LaboratoryWorkerId { get; set; }
    };
    [ApiController]
    [Route("api/lab/executeexamination")]
    public class executeExaminationController : ControllerBase
    {
        public IActionResult Post(examination cex)
        {
            using (var db = new DatabaseContext())
            {

                var ex = db.LaboratoryExaminations.SingleOrDefault(x => x.LaboratoryExaminationsId == cex.LaboratoryExaminationsId);
                if (ex != null)
                {
                    ex.Status = "Executed";
                    ex.Result = cex.Result;
                    ex.LaboratoryWorkerId = cex.LaboratoryWorkerId;
                    ex.ExaminationDate = DateTime.Now;
                    db.SaveChanges();
                    return Ok();
                }
                return BadRequest();
            }
        }
    }
}
