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
    public class leapproval{
        public int laboratoryExaminationId { get; set; }
        public int laboratoryManagerId { get; set; }
    };
    [ApiController]
    [Route("api/labmanager/examinationapproval")]
    /*
    {
    "laboratoryExaminationId": ,
    "laboratoryManagerId": 
    }
    */
    public class examinationApprovalController : ControllerBase{
        public IActionResult Post(leapproval lea){
            using (var db = new DatabaseContext()){

                var ex = db.LaboratoryExaminations.SingleOrDefault(x => x.LaboratoryExaminationId == lea.laboratoryExaminationId);
                if (lea != null && ex.Status == "Executed"){
                    ex.Status = "Approval";
                    ex.ApprovalRejectionDate = DateTime.Now;
                    ex.LaboratoryManagerId = lea.laboratoryManagerId;
                    db.SaveChanges();
                    return Ok();
                } return BadRequest();
            }
        }
    }
}