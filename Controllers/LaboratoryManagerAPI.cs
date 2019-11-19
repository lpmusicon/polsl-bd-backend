using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using BackendProject.Interface;

namespace BackendProject.Controllers
{

    [ApiController]
    [Route("laboratory_manager/examination_approval")]
    /*
    {
    "LaboratoryExaminationId": ,
    "LaboratoryManagerId": 
    }
    */
    public class ExaminationApprovalController : ControllerBase
    {
        public IActionResult Post(ExaminationApproval input)
        {
            using var db = new DatabaseContext();
            var ex = db.LaboratoryExaminations.SingleOrDefault(x => x.LaboratoryExaminationId == input.LaboratoryExaminationId);
            if (ex != null && ex.Status == "Executed")
            {
                ex.Status = "Approval";
                ex.ApprovalRejectionDate = DateTime.Now;
                ex.LaboratoryManagerId = input.LaboratoryManagerId;
                db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }
    }

    [ApiController]
    [Route("laboratory_manager/examination_reject")]
    /*
    {
    "LaboratoryExaminationId": ,
    "LaboratoryManagerId": ,
    "ManagerComment": ""
    }
    */
    public class ExaminationRejectController : ControllerBase
    {
        public IActionResult Post(ExaminationReject input)
        {
            using var db = new DatabaseContext();
            var ex = db.LaboratoryExaminations.SingleOrDefault(x => x.LaboratoryExaminationId == input.LaboratoryExaminationId);
            if (ex != null && ex.Status == "Executed" && input.ManagerComment != null)
            {
                ex.Status = "Rejected";
                ex.ApprovalRejectionDate = DateTime.Now;
                ex.ManagerComment = input.ManagerComment;
                ex.LaboratoryManagerId = input.LaboratoryManagerId;
                db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }
    }

    [ApiController]
    [Route("laboratory_manager/get_executed_examinations_list")]
    [Route("laboratory_worker/get_executed_examinations_list")]
    public class GetExecutedExaminationsListController : ControllerBase
    {
        public string Get()
        {
            using var db = new DatabaseContext();
            var result = (from le in db.LaboratoryExaminations
                          join lw in db.LaboratoryWorkers on le.LaboratoryWorkerId equals lw.LaboratoryWorkerId
                          where le.Status == "Executed"
                          select new ExecutedExaminationList
                          {
                              Result = le.Result,
                              DoctorComment = le.DoctorComment,
                              OrderDate = le.OrderDate,
                              ExaminationDate = le.ExaminationDate,
                              Status = le.Status,
                              LabWorkerName = lw.Name,
                              LabWorkerLastname = lw.Lastname
                          }).ToList();

            return JsonSerializer.Serialize<List<ExecutedExaminationList>>(result);
        }
        [HttpGet("{LaboratoryWorkerId}")]
        public string Get(int LaboratoryWorkerId)
        { // to na wypadek gdyby pracownik laboratorium mogl przegladac tylko swoje wykonane badania, jak moze wszystkie to wyrzucic
            using var db = new DatabaseContext();
            var result = (from le in db.LaboratoryExaminations
                          join lw in db.LaboratoryWorkers on le.LaboratoryWorkerId equals lw.LaboratoryWorkerId
                          where le.Status == "Executed" && le.LaboratoryWorkerId == LaboratoryWorkerId
                          select new ExecutedExaminationList
                          {
                              Result = le.Result,
                              DoctorComment = le.DoctorComment,
                              OrderDate = le.OrderDate,
                              ExaminationDate = le.ExaminationDate,
                              Status = le.Status,
                              LabWorkerName = lw.Name,
                              LabWorkerLastname = lw.Lastname
                          }).ToList();

            return JsonSerializer.Serialize<List<ExecutedExaminationList>>(result);
        }
    }

    [ApiController]
    [Route("laboratory_manager/get_resolved_examinations_list")]
    public class GetResolvedExaminationsListController : ControllerBase
    {
        public string Get()
        {
            using var db = new DatabaseContext();
            var result = (from le in db.LaboratoryExaminations
                          join lw in db.LaboratoryWorkers on le.LaboratoryWorkerId equals lw.LaboratoryWorkerId
                          join lm in db.LaboratoryManagers on le.LaboratoryManagerId equals lm.LaboratoryManagerId
                          where le.Status == "Approval" || le.Status == "Rejected"
                          select new ResolvedExaminationList
                          {
                              Result = le.Result,
                              DoctorComment = le.DoctorComment,
                              OrderDate = le.OrderDate,
                              ExaminationDate = le.ExaminationDate,
                              Status = le.Status,
                              LabWorkerName = lw.Name,
                              LabWorkerLastname = lw.Lastname,
                              LabManagerName = lm.Name,
                              LabManagerLastname = lm.Lastname,
                              ApprovalRejectionDate = le.ApprovalRejectionDate
                          }).ToList();

            return JsonSerializer.Serialize<List<ResolvedExaminationList>>(result);
        }
    }
}