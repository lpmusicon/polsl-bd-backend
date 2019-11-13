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
        public int laboratoryExaminationId { get; set; }
        public string result { get; set; }
        public int laboratoryWorkerId { get; set; }
    };
    [ApiController]
    [Route("api/lab/executeexamination")]
    /*
    {
    "laboratoryExaminationId": ,
    "result": "",
    "laboratoryWorkerId": 
    }
    */
    public class executeExaminationController : ControllerBase
    {
        public IActionResult Post(examination cex)
        {
            using (var db = new DatabaseContext())
            {

                var ex = db.LaboratoryExaminations.SingleOrDefault(x => x.LaboratoryExaminationId == cex.laboratoryExaminationId);
                if (ex != null)
                {
                    ex.Status = "Executed";
                    ex.Result = cex.result;
                    ex.LaboratoryWorkerId = cex.laboratoryWorkerId;
                    ex.ExaminationDate = DateTime.Now;
                    db.SaveChanges();
                    return Ok();
                }
                return BadRequest();
            }
        }
    }
}