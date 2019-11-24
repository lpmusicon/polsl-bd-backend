using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using BackendProject.Models;
using Microsoft.AspNetCore.Authorization;

namespace BackendProject.Controllers
{
    [ApiController]
    [Route("patient")]
    public class PatientController : ControllerBase
    {
        [HttpGet]
        [HttpGet("all")]
        [Authorize(Roles = "RECP, DOCT")]
        public List<Patient> All()
        {
            return new DatabaseContext().Patients.ToList();
        }

        /*
        {
            "Name": "",
            "Lastname": "",
            "PESEL": ""
        }
        */
        [HttpPost("register")]
        [Authorize(Roles = "RECP")]
        public IActionResult Register(PatientRegisterModel input)
        {
            using var db = new DatabaseContext();
            // tutaj sprawdzanie czy login jest unikalny
            if (db.Patients.SingleOrDefault(x => x.PESEL == input.PESEL) != null)
                return BadRequest("Duplicate PESEL");

            if (input.PESEL.All(char.IsDigit))
            {
                Patient p = new Patient()
                {
                    PESEL = input.PESEL,
                    Name = input.Name,
                    Lastname = input.Lastname
                };
                db.Patients.Add(p);
                db.SaveChanges();
                return NoContent();
            }
            return BadRequest();
        }

        [HttpGet("{patientId}/visit")]
        [HttpGet("{patientId}/visit/all")]
        [Authorize(Roles = "RECP, DOCT")]
        public List<PatientVisitsList> Visits(int patientId)
        {
            using var db = new DatabaseContext();
            var result = (
                from p in db.Patients
                join pv in db.PatientVisits on p.PatientId equals pv.PatientId
                join d in db.Doctors on pv.DoctorId equals d.DoctorId
                where pv.PatientId == patientId
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
                    PatientId = patientId
                }).ToList();

            return result;
        }
    }
}