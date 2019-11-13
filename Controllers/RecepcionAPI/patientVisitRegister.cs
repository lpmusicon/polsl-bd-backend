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
    [Route("api/recepcion/patientvisitregister")]
    public class patientVisitRegisterController : ControllerBase{
        public IActionResult Post(PatientVisit npv){
            
            using (var db = new DatabaseContext()){

                var patientExist = db.Patients.SingleOrDefault(x => x.PatientId == npv.PatientId);
                if(patientExist == null) return BadRequest();

                if(npv.PatientVisitId == 0 && npv.Description == null && npv.Diagnosis == null && npv.CloseDate == null){
                    db.PatientVisits.Add(npv);
                    db.SaveChanges();
                    return StatusCode(201);
                }else return BadRequest();
            }
        }
    }
}
