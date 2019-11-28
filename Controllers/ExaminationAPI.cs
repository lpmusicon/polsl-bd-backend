using System.Net;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using BackendProject.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using BackendProject.Models.Laboratory;
using BackendProject.Models.Physical;

namespace BackendProject.Controllers
{
    [ApiController]
    [Route("examination")]
    public class ExaminationController : ControllerBase
    {
        public string UserId => User.Identity.Name;

        [HttpGet("dictionary")]
        [HttpGet("dictionary/all")]
        public List<DictionaryModel> DictionaryAll()
        {
            using var db = new DatabaseContext();
            var result = (from x in db.ExaminationsDictionary
                          select new DictionaryModel
                          {
                              Id = x.ExaminationDictionaryId,
                              Name = x.Name,
                              Type = x.Type == 'L' ? "Laboratory" : "Physical"
                          }).ToList();
            return result;
        }

        [HttpGet("dictionary/laboratory")]
        public List<DictionaryModel> DictionaryLaboratory()
        {
            using var db = new DatabaseContext();
            var result = (from x in db.ExaminationsDictionary
                          where x.Type == 'L'
                          select new DictionaryModel
                          {
                              Id = x.ExaminationDictionaryId,
                              Name = x.Name,
                              Type = x.Type == 'L' ? "Laboratory" : "Physical"
                          }).ToList();
            return result;
        }

        [HttpGet("dictionary/physical")]
        [Authorize(Roles = "DOCT")]
        public List<DictionaryModel> DictionaryPhysical()
        {
            using var db = new DatabaseContext();
            var result = (from x in db.ExaminationsDictionary
                          where x.Type == 'F'
                          select new DictionaryModel
                          {
                              Id = x.ExaminationDictionaryId,
                              Name = x.Name,
                              Type = x.Type == 'L' ? "Laboratory" : "Physical"
                          }).ToList();
            return result;
        }

        /* 
        {
            "ExaminationDictionaryId": , 
            "PatientVisitId": ,
            "Result": ""  
        }
        */
        [HttpPost("physical/perform")]
        [Authorize(Roles = "DOCT")]
        public IActionResult Perform(PerformExaminationModel input)
        {
            using var db = new DatabaseContext();
            var pvcheck = db.PatientVisits.SingleOrDefault(x => x.PatientVisitId == input.VisitId);
            var pecheck = db.ExaminationsDictionary.SingleOrDefault(x => x.ExaminationDictionaryId == input.ExaminationTypeId);
            bool isExaminationValid = pvcheck != null && pvcheck.Status == "Registered" && pecheck.Type == 'F';
            if (isExaminationValid)
            {
                var PE = new PhysicalExamination()
                {
                    Result = input.Result,
                    PatientVisitId = input.VisitId,
                    ExaminationDictionaryId = input.ExaminationTypeId
                };
                db.Add(input);
                db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        /* Zbiera badania fizykalne z wizyty */
        [HttpGet("physical/performed/{visitId}")]
        public List<PhysicalExaminationModel> PhysicalExamination(int visitId)
        {
            using var db = new DatabaseContext();
            var result = (from pe in db.PhysicalExaminations
                          join ed in db.ExaminationsDictionary on pe.ExaminationDictionaryId equals ed.ExaminationDictionaryId
                          where pe.PatientVisitId == visitId
                          select new PhysicalExaminationModel
                          {
                              Name = ed.Name,
                              Result = pe.Result
                          }).ToList();

            return result;
        }


        [HttpGet("laboratory/all")]
        public List<Resolved> LaboratoryAll()
        {
            using var db = new DatabaseContext();
            var result = (from le in db.LaboratoryExaminations
                          join lw in db.LaboratoryWorkers on le.LaboratoryWorkerId equals lw.LaboratoryWorkerId
                          join lm in db.LaboratoryManagers on le.LaboratoryManagerId equals lm.LaboratoryManagerId
                          select new Resolved
                          {
                              Result = le.Result,
                              DoctorComment = le.DoctorComment,
                              OrderDate = le.OrderDate,
                              ExaminationDate = le.ExaminationDate,
                              Status = le.Status,
                              Worker = new Person() {
                                  Id = lw.LaboratoryWorkerId,
                                  Name = lw.Name,
                                  Lastname = lw.Lastname
                              },
                              Manager = new Person() {
                                  Id = lm.LaboratoryManagerId,
                                  Name = lm.Name,
                                  Lastname = lm.Lastname
                              },
                              ApprovalRejectionDate = le.ApprovalRejectionDate
                          }).ToList();
            return result;
        }

        [HttpGet("laboratory/ordered")]
        public List<Ordered> LaboratoryOrdered()
        {
            using var db = new DatabaseContext();
            var result = (from le in db.LaboratoryExaminations
                          join ed in db.ExaminationsDictionary
                          on le.ExaminationDictionaryId equals ed.ExaminationDictionaryId
                          where le.Status == "Ordered"
                          select new Ordered
                          {
                              Id = le.LaboratoryExaminationId,
                              LaboratoryExaminationName = ed.Name,
                              DoctorComment = le.DoctorComment,
                              OrderDate = le.OrderDate,
                          }).ToList();
            return result;
        }

        /* Zbiera badania laboratoryjne z wizyty */
        [HttpGet("laboratory/ordered/{visitId}")]
        public List<OrderedExamination> Ordered(int visitId)
        {
            using var db = new DatabaseContext();
            var result = (from le in db.LaboratoryExaminations
                          join ed in db.ExaminationsDictionary on le.ExaminationDictionaryId equals ed.ExaminationDictionaryId
                          where le.PatientVisitId == visitId
                          select new OrderedExamination
                          {
                              Id = le.LaboratoryExaminationId,
                              Result = le.Result,
                              DoctorComment = le.DoctorComment,
                              ExaminationDate = le.ExaminationDate,
                              Status = le.Status,
                              ManagerComment = le.ManagerComment,
                              ExaminationName = ed.Name
                          }).ToList();

            return result;
        }

        [HttpGet("laboratory/pending")]
        public List<Executed> Pending()
        {
            using var db = new DatabaseContext();
            var result = (from le in db.LaboratoryExaminations
                          join lw in db.LaboratoryWorkers on le.LaboratoryWorkerId equals lw.LaboratoryWorkerId
                          where le.Status == "Executed"
                          select new Executed
                          {
                              Result = le.Result,
                              DoctorComment = le.DoctorComment,
                              OrderDate = le.OrderDate,
                              ExaminationDate = le.ExaminationDate,
                              Status = le.Status,
                              Worker = new Person() {
                                  Id = lw.LaboratoryWorkerId,
                                  Name = lw.Name,
                                  Lastname = lw.Lastname
                              }
                          }).ToList();

            return result;
        }

        /* 
        {
            "DoctorComment": "", 
            "PatientVisitId": , 
            "ExaminationDictionaryId":  
        }
        */
        [HttpPost("laboratory/order")]
        public IActionResult Order(OrderExaminationModel input)
        {
            
            using var db = new DatabaseContext();
            var pecheck = db.ExaminationsDictionary.SingleOrDefault(x => x.ExaminationDictionaryId == input.ExaminationTypeId);
            if (db.PatientVisits.SingleOrDefault(x => x.PatientVisitId == input.VisitId) != null && pecheck.Type == 'L')
            {
                var Examination = new LaboratoryExamination() {
                    PatientVisitId = input.VisitId,
                    DoctorComment = input.DoctorComment,
                    OrderDate = DateTime.Now,
                    Status = "Ordered",
                    ExaminationDictionaryId = input.ExaminationTypeId
                };
                db.Add(Examination);
                db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        /*
        {
            "Result": "",
        }
        */
        [HttpPost("laboratory/{examinationId}/do")]
        [Authorize(Roles = "LABW")]
        public IActionResult LaboratoryDo(int examinationId, ResultModel input)
        {
            var uid = int.Parse(UserId);
            using var db = new DatabaseContext();
            var ex = db.LaboratoryExaminations.SingleOrDefault(x => x.LaboratoryExaminationId == examinationId);
            if (ex != null && input.Result != null)
            {
                ex.Status = "Executed";
                ex.Result = input.Result;
                ex.LaboratoryWorkerId = uid;
                ex.ExaminationDate = DateTime.Now;
                db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        /*
        {
            "LaboratoryWorkerId": 
        }
        */
        [HttpPost("laboratory/{examinationId}/abort")]
        [Authorize(Roles = "LABW")]
        public IActionResult LaboratoryAbort(int examinationId, ReasonModel input)
        {
            var uid = int.Parse(UserId);
            using var db = new DatabaseContext();
            var ex = db.LaboratoryExaminations.SingleOrDefault(x => x.LaboratoryExaminationId == examinationId);
            if (ex != null)
            {
                ex.ManagerComment = input.Reason;
                ex.Status = "Canceled";
                ex.ExaminationDate = DateTime.Now;
                ex.LaboratoryWorkerId = uid;
                db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("laboratory/{examinationId}/approve")]
        [Authorize(Roles = "LABM")]
        public IActionResult Approve(int examinationId, ReasonModel input)
        {
            var uid = int.Parse(UserId);
            using var db = new DatabaseContext();
            var ex = db.LaboratoryExaminations.SingleOrDefault(x => x.LaboratoryExaminationId == examinationId);
            if (ex != null && ex.Status == "Executed")
            {
                ex.ManagerComment = input.Reason;
                ex.Status = "Approval";
                ex.ApprovalRejectionDate = DateTime.Now;
                ex.LaboratoryManagerId = uid;
                db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("laboratory/{examinationId}/reject")]
        [Authorize(Roles = "LABM")]
        public IActionResult Reject(int examinationId, ReasonModel input)
        {
            var uid = int.Parse(UserId);
            using var db = new DatabaseContext();
            var ex = db.LaboratoryExaminations.SingleOrDefault(x => x.LaboratoryExaminationId == examinationId);
            if (ex != null && ex.Status == "Executed" && input.Reason != null)
            {
                ex.Status = "Rejected";
                ex.ApprovalRejectionDate = DateTime.Now;
                ex.ManagerComment = input.Reason;
                ex.LaboratoryManagerId = uid;
                db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }
    }
}