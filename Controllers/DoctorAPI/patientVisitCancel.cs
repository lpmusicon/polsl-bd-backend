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

    [ApiController]
    [Route("api/doctor/patientvisitcancel")]
    /* 
    {
    "patientVisitId": , 
    "description": ""
    }
    */
    public class patientVisitDoctorCancelController : ControllerBase{
        public IActionResult Post(pvcancel cpv){
            using (var db = new DatabaseContext()){
                // anuluje tylko swoje wizyty? jesli tak, dopisz cos, teraz moze anulowac wszystko (Kononowicz mode)
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
