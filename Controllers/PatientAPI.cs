using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
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
        public string All()
        {
            using var db = new DatabaseContext();
            return JsonSerializer.Serialize<DbSet<Patient>>(db.Patients);
        }

        /*
        {
            "Name": "",
            "Lastname": "",
            "PESEL": ""
        }
        */
        [HttpPost("register")]
        [Authorize(Roles="RECP")]
        public IActionResult Register(Patient input)
        {
            using var db = new DatabaseContext();
            // tutaj sprawdzanie czy login jest unikalny
            if (db.Patients.SingleOrDefault(x => x.PESEL == input.PESEL) != null)
                return BadRequest();

            if (input.PatientId == 0 && input.PESEL.Length == 11 && input.PESEL.All(char.IsDigit))
            {
                db.Patients.Add(input);
                db.SaveChanges();
                return StatusCode(201);
            }
            return BadRequest();
        }

        [HttpGet("{patientId}/visit")]
        [HttpGet("{patientId}/visit/all")]
        [Authorize(Roles="RECP, DOCT")]
        public IActionResult Visits(int patientId)
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

            if (result.Count == 0)
            {
                return NotFound();
            }

            return Ok(JsonSerializer.Serialize<List<PatientVisitsList>>(result));
        }
    }
}