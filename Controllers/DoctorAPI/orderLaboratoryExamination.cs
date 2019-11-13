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
    [Route("api/doctor/orderlaboratoryexamination")]
    /* 
    {
    "DoctorComment": "", 
    "PatientVisitId": , 
    "ExaminationDictionaryId":  
    }
    */
    public class orderLaboratoryExaminationController : ControllerBase{
        public IActionResult Post(LaboratoryExamination nle){
            
            using (var db = new DatabaseContext()){
                var pecheck = db.ExaminationsDictionary.SingleOrDefault(x => x.ExaminationDictionaryId == nle.ExaminationDictionaryId);
                if(nle.LaboratoryExaminationId == 0 && db.PatientVisits.SingleOrDefault(x => x.PatientVisitId == nle.PatientVisitId) != null && pecheck.Type == 'L' && 
                nle.ManagerComment == null && nle.ApprovalRejectionDate == null && nle.Result == null){
                    nle.OrderDate = DateTime.Now;
                    nle.Status = "Ordered";
                    db.Add(nle);
                    db.SaveChanges();
                    return Ok();
                }else return BadRequest();
            } 
        }
    }
}
