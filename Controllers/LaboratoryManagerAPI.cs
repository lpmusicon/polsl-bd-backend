using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using BackendProject.Models;

namespace BackendProject.Controllers
{

    [ApiController]
    [Route("laboratory_manager")]
    public class LaboratoryManagerController : ControllerBase
    {
        [HttpGet]
        [HttpGet("all")]
        public string All()
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
        [HttpGet("pending")]
        public string GetPending()
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

        [HttpGet("{LaboratoryWorkerId}/pending")]
        public string GetPending(int LaboratoryWorkerId)
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

        /*
        {
            "LaboratoryExaminationId": ,
            "LaboratoryManagerId": 
        }
        */
        [HttpPost("{LaboratoryExaminationId}/approval")]
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

        /*
        {
            "LaboratoryExaminationId": ,
            "LaboratoryManagerId": ,
            "ManagerComment": ""
        }
        */
        [HttpPost("{LaboratoryExaminationId}/reject")]
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
}