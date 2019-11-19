using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using BackendProject.Models;

namespace BackendProject.Controllers
{
    [ApiController]
    [Route("doctor")]
    public class DoctorController : ControllerBase
    {
        [HttpGet]
        [HttpGet("all")]
        public List<DoctorsList> All()
        {
            using var db = new DatabaseContext();
            var result = (from x in db.Doctors select new DoctorsList { 
                DoctorId = x.DoctorId, 
                Name = x.Name, 
                Lastname = x.Lastname }).ToList();

            return result;
        }

        /**
            Zbiera wizyty zarejestrowane dla doktora
        */
        [HttpGet("{doctorId}/visits/registered")]
        public List<PatientsVisitsList> RegisteredVisits(int doctorId)
        {
            using var db = new DatabaseContext();
            var result = (from p in db.Patients
                          join pv in db.PatientVisits on p.PatientId equals pv.PatientId
                          where pv.DoctorId == doctorId && pv.Status == "Registered"
                          select new PatientsVisitsList
                          {
                              PatientName = p.Name,
                              PatientLastname = p.Lastname,
                              RegisterDate = pv.RegisterDate,
                              PatientId = pv.PatientId
                          }).ToList();

            return result;
        }
    }
}
