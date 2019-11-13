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
    public class lecancel{
        public int laboratoryExaminationId { get; set; }
        public int laboratoryManagerId { get; set; }
        public string managerComment { get; set; }
    };
    [ApiController]
    [Route("api/labmanager/examinationreject")]
    /*
    {
    "laboratoryExaminationId": ,
    "laboratoryManagerId": ,
    "managerComment": ""
    }
    */
    public class examinationRejectController : ControllerBase{
        public IActionResult Post(lecancel lec){
            using (var db = new DatabaseContext()){

                var ex = db.LaboratoryExaminations.SingleOrDefault(x => x.LaboratoryExaminationId == lec.laboratoryExaminationId);
                if (lec != null && ex.Status == "Executed" && lec.managerComment != null){
                    ex.Status = "Rejected";
                    ex.ApprovalRejectionDate = DateTime.Now;
                    ex.ManagerComment = lec.managerComment;
                    ex.LaboratoryManagerId = lec.laboratoryManagerId;
                    db.SaveChanges();
                    return Ok();
                } return BadRequest();
            }
        }
    }
}