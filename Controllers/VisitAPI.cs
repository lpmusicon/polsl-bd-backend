using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using BackendProject.Models;
using Microsoft.AspNetCore.Authorization;

namespace BackendProject.Controllers
{
    [ApiController]
    [Route("visit")]
    public class VisitController : ControllerBase
    {
        public string UserId => User.Identity.Name;

        [HttpGet]
        [HttpGet("all")]
        public string All()
        {
            using var db = new DatabaseContext();
            var result = (from p in db.Patients
                          join pv in db.PatientVisits on p.PatientId equals pv.PatientId
                          join d in db.Doctors on pv.DoctorId equals d.DoctorId
                          select new PatientVisitsModel
                          {
                              Patient = new PatientModel() {
                                  Id = p.PatientId,
                                  Name = p.Name,
                                  Lastname = p.Lastname
                              },
                              Doctor = new DoctorModel() {
                                  Id = d.DoctorId,
                                  Name = d.Name,
                                  Lastname = d.Lastname
                              },
                              RegisterDate = pv.RegisterDate,
                              PatientVisitId = pv.PatientVisitId,
                              CloseDate = pv.CloseDate,
                              Description = pv.Description,
                              Diagnosis = pv.Diagnosis,
                              Status = pv.Status
                          }).ToList();

            return JsonSerializer.Serialize<List<PatientVisitsModel>>(result);
        }

        [HttpGet("registered")]
        [HttpGet("registered/all")]
        public List<AllPatientsVisitsModel> AllRegistered()
        {
            using var db = new DatabaseContext();
            var result = (from p in db.Patients
                          join pv in db.PatientVisits on p.PatientId equals pv.PatientId
                          join d in db.Doctors on pv.DoctorId equals d.DoctorId
                          where pv.Status == "Registered"
                          select new AllPatientsVisitsModel
                          {
                              Id = pv.PatientVisitId,
                              Doctor = new DoctorModel() {
                                  Id = d.DoctorId,
                                  Name = d.Name,
                                  Lastname = d.Lastname
                              },
                              Patient = new PatientModel() {
                                  Id = p.PatientId,
                                  Name = p.Name,
                                  Lastname = p.Lastname
                              },
                              RegisterDate = pv.RegisterDate
                          }).ToList();
            return result;
        }

        [HttpGet("{visitId}")]
        public VisitModel Visit(int visitId)
        {
            using var db = new DatabaseContext();
            var visit = db.PatientVisits.SingleOrDefault(x => x.PatientVisitId == visitId);
            if(visit == null) return new VisitModel();
            return new VisitModel() {
                Id = visit.PatientVisitId,
                RegisterDate = visit.RegisterDate,
                Patient = new PatientModel() {}
            };
        }

        /*
        {
            "PatientId": ,
            "DoctorId": ,
            "ReceptionistId": 
        }
        */
        [HttpPost("register")]
        public IActionResult Register(PatientVisit input)
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
        public IActionResult Cancel(int visitId, ReasonModel formData)
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
        [Authorize(Roles = "DOCT")]
        public IActionResult Close(int visitId, PatientVisitModel input)
        {
            var uid = int.Parse(UserId);
            bool isInputValid = visitId != 0 && uid != 0 && input.Description != null && input.Diagnosis != null;
            if (isInputValid)
            {
                using var db = new DatabaseContext();
                var pv = db.PatientVisits.SingleOrDefault(x => x.PatientVisitId == visitId);
                if (pv != null && pv.Status == "Registered" && db.Doctors.SingleOrDefault(x => x.DoctorId == uid) != null)
                {
                    pv.Diagnosis = input.Diagnosis;
                    pv.Description = input.Description;
                    pv.DoctorId = uid;
                    pv.CloseDate = DateTime.Now;
                    pv.Status = "Closed";
                    db.SaveChanges();
                    return Ok();
                }
                return NotFound();
            }
            return BadRequest();
        }

    }
}