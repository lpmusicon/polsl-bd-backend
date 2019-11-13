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
    [Route("api/doctor/physicalexamination")]
    /* 
    {
    "ExaminationDictionaryId": , 
    "PatientVisitId": ,
    "Result": ""  
    }
    */
    public class psychicalExaminationController : ControllerBase{
        public IActionResult Post(PhysicalExamination npe){
            
            using (var db = new DatabaseContext()){
                var pvcheck = db.PatientVisits.SingleOrDefault(x => x.PatientVisitId == npe.PatientVisitId);
                var pecheck = db.ExaminationsDictionary.SingleOrDefault(x => x.ExaminationDictionaryId == npe.ExaminationDictionaryId);
                if(npe.PhysicalExaminationId == 0 && pvcheck != null && pvcheck.Status == "Registered" && pecheck.Type == 'F'){
                    db.Add(npe);
                    db.SaveChanges();
                    return Ok();
                }else return BadRequest();
            } 
        }
    }
}
