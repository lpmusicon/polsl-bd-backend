using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using BackendProject.Models;

namespace BackendProject.Controllers
{
    [ApiController]
    [Route("visit")]
    public class VisitController : ControllerBase
    {
        [HttpGet]
        [HttpGet("all")]
        public string All()
        {
            using var db = new DatabaseContext();
            var result = (from p in db.Patients
                          join pv in db.PatientVisits on p.PatientId equals pv.PatientId
                          join d in db.Doctors on pv.DoctorId equals d.DoctorId
                          select new PatientVisitsList
                          {
                              PatientName = p.Name,
                              PatientLastname = p.Lastname,
                              RegisterDate = pv.RegisterDate,
                              PatientVisitId = pv.PatientVisitId,
                              CloseDate = pv.CloseDate,
                              Description = pv.Description,
                              Diagnosis = pv.Diagnosis,
                              DoctorName = d.Name,
                              DoctorLastname = d.Lastname,
                              Status = pv.Status,
                              PatientId = pv.PatientId
                          }).ToList();

            return JsonSerializer.Serialize<List<PatientVisitsList>>(result);
        }

        [HttpGet("registered")]
        [HttpGet("registered/all")]
        public List<AllPatientsVisitsList> AllRegistered()
        {
            using var db = new DatabaseContext();
            var result = (from p in db.Patients
                          join pv in db.PatientVisits on p.PatientId equals pv.PatientId
                          join d in db.Doctors on pv.DoctorId equals d.DoctorId
                          where pv.Status == "Registered"
                          select new AllPatientsVisitsList
                          {
                              DoctorName = d.Name,
                              DoctorLastname = d.Lastname,
                              PatientName = p.Name,
                              PatientLastname = p.Lastname,
                              RegisterDate = pv.RegisterDate,
                              PatientId = pv.PatientId
                          }).ToList();

            return result;
        }

        /*
        {
            "PatientId": ,
            "DoctorId": ,
            "ReceptionistId": 
        }
        */
        [HttpPost("register")]
        public IActionResult Post(PatientVisit input)
        {

            using var db = new DatabaseContext();
            if (db.Patients.SingleOrDefault(x => x.PatientId == input.PatientId) == null)
                return BadRequest();

            var isValid = input.PatientVisitId == 0 && input.Description == null && input.Diagnosis == null && input.CloseDate == null;
            if (isValid)
            {
                input.Status = "Registered";
                input.RegisterDate = DateTime.Now;
                db.PatientVisits.Add(input);
                db.SaveChanges();
                return StatusCode(201);
            }
            return BadRequest();
        }

        /* 
        {
            "PatientVisitId": , 
            "Description": ""
        }
        */
        [HttpPost("{visitId}/cancel")]
        public IActionResult Post(int visitId, VisitCancelModel formData)
        {
            using var db = new DatabaseContext();
            // anuluje tylko swoje wizyty? jesli tak, dopisz cos, teraz moze anulowac wszystko (Kononowicz mode)
            var pv = db.PatientVisits.SingleOrDefault(x => x.PatientVisitId == visitId);

            if (pv != null && formData.Reason != null)
            {
                pv.Status = "Canceled";
                pv.CloseDate = DateTime.Now;
                pv.Description = formData.Reason;
                db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        /* 
        {
            "PatientVisitId": , 
            "DoctorID": , 
            "Description": "",
            "Diagnosis": ""  
        }
        */
        [HttpPost("{visitId}/close")]
        public IActionResult Post(PatientVisitForm input)
        {
            bool isInputValid = input.PatientVisitId != 0 && input.DoctorId != 0 && input.Description != null && input.Diagnosis != null;
            if (isInputValid)
            { // chyba musial wpisac opis, dowiemy sie
                // Biere login
                using var db = new DatabaseContext();
                var pv = db.PatientVisits.SingleOrDefault(x => x.PatientVisitId == input.PatientVisitId);
                if (pv != null && pv.Status == "Registered" && db.Doctors.SingleOrDefault(x => x.DoctorId == input.DoctorId) != null)
                {
                    pv.Diagnosis = input.Diagnosis;
                    pv.Description = input.Description;
                    pv.DoctorId = input.DoctorId;
                    pv.CloseDate = DateTime.Now;
                    pv.Status = "Closed";
                    db.SaveChanges();
                    return Ok();
                }
                return NotFound();
            }
            return BadRequest();
        }

        /* 
        {
            "ExaminationDictionaryId": , 
            "PatientVisitId": ,
            "Result": ""  
        }
        */
        [HttpPost("{visitId}/examination/perform")]
        public IActionResult Post(int visitId, PhysicalExamination input)
        {
            using var db = new DatabaseContext();
            var pvcheck = db.PatientVisits.SingleOrDefault(x => x.PatientVisitId == input.PatientVisitId);
            var pecheck = db.ExaminationsDictionary.SingleOrDefault(x => x.ExaminationDictionaryId == input.ExaminationDictionaryId);
            bool isExaminationValid = input.PhysicalExaminationId == 0 && pvcheck != null && pvcheck.Status == "Registered" && pecheck.Type == 'F';
            if (isExaminationValid)
            {
                db.Add(input);
                db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        /* 
        {
            "DoctorComment": "", 
            "PatientVisitId": , 
            "ExaminationDictionaryId":  
        }
        */
        [HttpPost("{visitId}/examination/order")]
        public IActionResult Post(LaboratoryExamination input)
        {

            using var db = new DatabaseContext();
            var pecheck = db.ExaminationsDictionary.SingleOrDefault(x => x.ExaminationDictionaryId == input.ExaminationDictionaryId);
            if (input.LaboratoryExaminationId == 0 && db.PatientVisits.SingleOrDefault(x => x.PatientVisitId == input.PatientVisitId) != null && pecheck.Type == 'L' &&
            input.ManagerComment == null && input.ApprovalRejectionDate == null && input.Result == null)
            {
                input.OrderDate = DateTime.Now;
                input.Status = "Ordered";
                db.Add(input);
                db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        /* Zbiera badania laboratoryjne z wizyty */
        [HttpGet("{visitId}/examination/ordered")]
        public List<LaboratoryExaminationList> GetVisit(int PatientVisitId)
        {
            using var db = new DatabaseContext();
            var result = (from le in db.LaboratoryExaminations
                          join ed in db.ExaminationsDictionary on le.ExaminationDictionaryId equals ed.ExaminationDictionaryId
                          where le.PatientVisitId == PatientVisitId
                          select new LaboratoryExaminationList
                          {
                              Result = le.Result,
                              DoctorComment = le.DoctorComment,
                              ExaminationDate = le.ExaminationDate,
                              Status = le.Status,
                              ManagerComment = le.ManagerComment,
                              ExaminationName = ed.Name
                          }).ToList();

            return result;
        }

        /* Zbiera badania fizykalne z wizyty */
        [HttpGet("{visitId}/examination/performed")]
        public List<PhysicalExaminationList> GetPhysicalExamination(int PatientVisitId)
        {
            using var db = new DatabaseContext();
            var result = (from pe in db.PhysicalExaminations
                          join ed in db.ExaminationsDictionary on pe.ExaminationDictionaryId equals ed.ExaminationDictionaryId
                          where pe.PatientVisitId == PatientVisitId
                          select new PhysicalExaminationList { ExaminationName = ed.Name, Result = pe.Result }).ToList();

            return result;
        }
    }
}