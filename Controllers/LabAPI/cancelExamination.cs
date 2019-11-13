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
    public class excancel
    {
        public int LaboratoryExaminationsId { get; set; }
        public int LaboratoryWorkerId { get; set; }
    };
    [ApiController]
    [Route("api/lab/cancelexamination")]
    public class cancelExaminationController : ControllerBase
    {
        public IActionResult Post(excancel cex)
        {
            using (var db = new DatabaseContext())
            {

                var ex = db.LaboratoryExaminations.SingleOrDefault(x => x.LaboratoryExaminationsId == cex.LaboratoryExaminationsId);
                if (ex != null)
                {
                    ex.Status = "Canceled";
                    ex.ExaminationDate = DateTime.Now;
                    ex.LaboratoryWorkerId = cex.LaboratoryWorkerId;
                    db.SaveChanges();
                    return Ok();
                }
                return BadRequest();
            }
        }
    }
}
