using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using BackendProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace BackendProject.Controllers
{
    [ApiController]
    [Route("visit")]
    public class VisitController : ControllerBase
    {
        public string UserId => User.Identity.Name;

        private readonly ILogger<VisitController> _logger;

        public VisitController(ILogger<VisitController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [HttpGet("all")]
        [Authorize(Roles="RECP, DOCT")]
        public string All()
        {
            using var db = new DatabaseContext();
            var result = (from p in db.Patients
                          join pv in db.PatientVisits on p.PatientId equals pv.PatientId
                          join d in db.Doctors on pv.DoctorId equals d.DoctorId
                          select new PatientVisitsModel
                          {
                              Patient = new PatientModel() {
                                  PatientId = p.PatientId,
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
        [Authorize(Roles="RECP, DOCT")]
        public List<AllPatientsVisitsModel> AllRegistered() // do dogadania
        {
            using var db = new DatabaseContext();
            var result = (from p in db.Patients
                          join pv in db.PatientVisits on p.PatientId equals pv.PatientId
                          join d in db.Doctors on pv.DoctorId equals d.DoctorId
                          where pv.Status == "Registered"
                          select new AllPatientsVisitsModel
                          {
                              PatientVisitId = pv.PatientVisitId,
                              Doctor = new DoctorModel() {
                                  Id = d.DoctorId,
                                  Name = d.Name,
                                  Lastname = d.Lastname
                              },
                              Patient = new PatientModel() {
                                  PatientId = p.PatientId,
                                  Name = p.Name,
                                  Lastname = p.Lastname
                              },
                              RegisterDate = pv.RegisterDate
                          }).ToList();
            return result;
        }

        [HttpGet("{visitId}")]
        [Authorize(Roles = "DOCT, RECP")]
        public VisitModel Visit(int visitId) // strzelam ze to pobierane zeby wykonac wizyte, sprawdzanie doktora? in progress?
        {
            using var db = new DatabaseContext();
            var visit = db.PatientVisits.SingleOrDefault(x => x.PatientVisitId == visitId);
            if(visit == null) return new VisitModel();
            var patient = db.Patients.SingleOrDefault(x => x.PatientId == visit.PatientId); // do poprawy
            return new VisitModel() {
                PatientVisitId = visit.PatientVisitId,
                RegisterDate = visit.RegisterDate,
                Patient = new PatientModel() {
                    PatientId = patient.PatientId,
                    Name = patient.Name,
                    Lastname = patient.Lastname,
                    PESEL = patient.PESEL
                }
            };
        }

        /*
        {
            "PatientId": ,
            "DoctorId": ,
            "RegisterDate": 
        }
        */
        [HttpPost("register")]
        [Authorize(Roles="RECP")]
        public IActionResult Register(RegisterVisitModel input)
        {
            var UID = int.Parse(UserId);
            using var db = new DatabaseContext();

            if (input.RegisterDate > DateTime.Now && // sprawdzenie czy data wizyty wydarzy sie w przyszlosci
            db.Patients.SingleOrDefault(x => x.PatientId == input.PatientId) != null && // sprawdzenie czy istnieje taki pacjent i
            db.Doctors.SingleOrDefault(x => x.DoctorId == input.DoctorId) != null) // czy istnieje taki lekarz
            {    
                db.PatientVisits.Add(new PatientVisit{
                    Status = "Registered",
                    RegisterDate = input.RegisterDate,
                    PatientId = input.PatientId,
                    DoctorId = input.DoctorId,
                    ReceptionistId = UID
                });
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
        [Authorize(Roles="RECP, DOCT")]
        public IActionResult Cancel(int visitId, ReasonModel formData)
        {
            var UID = int.Parse(UserId);
            using var db = new DatabaseContext();

            var pv = db.PatientVisits.SingleOrDefault(x => x.PatientVisitId == visitId);

            if (pv != null && formData.Reason != null && pv.Status == "Registered" && 
            (User.IsInRole("DOCT") && UID == pv.DoctorId || User.IsInRole("RECP"))) // doktor moze anulowac tylko swoje a recp wszystkie
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
            "Description": "",
            "Diagnosis": ""  
        }
        */
        [HttpPost("{visitId}/close")]
        [Authorize(Roles = "DOCT")]
        public IActionResult Close(int VisitId, PatientVisitCloseModel input)
        {
            var UID = int.Parse(UserId);
            using var db = new DatabaseContext();
            var pv = db.PatientVisits.SingleOrDefault(x => x.PatientVisitId == VisitId);
                
            if (pv != null && pv.Status == "Registered" && pv.DoctorId == UID && pv.Description != null) // pv.Status == "In Progress" jesli bedziemy to robic, diagnoza moze byc null?
            {
                    pv.Diagnosis = input.Diagnosis;
                    pv.Description = input.Description;
                    pv.CloseDate = DateTime.Now;
                    pv.Status = "Closed";
                    db.SaveChanges();
                    return Ok();
            }
            return BadRequest();
        }
    }
}