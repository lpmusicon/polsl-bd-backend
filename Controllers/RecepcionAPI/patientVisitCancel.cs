using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization; 

namespace bd_backend.Controllers{
    public class pvcancel{
        public int PatientVisitId { get; set; }
        public string Description { get; set; }
    };
    [ApiController]
    [Route("api/recepcion/patientvisitcancel")]
    public class patientVisitCancelController : ControllerBase{
        public IActionResult Post(pvcancel cpv){
            using (var db = new DatabaseContext()){
                
                var pv = db.PatientVisits.SingleOrDefault(x => x.PatientVisitId == cpv.PatientVisitId);
                if(pv != null){
                    pv.Status = "Canceled";
                    pv.CloseDate = DateTime.Now;
                    pv.Description = cpv.Description;
                    db.SaveChanges();
                    return Ok();
                }return BadRequest();
            }
        }
    }
}
