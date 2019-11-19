using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using BackendProject.Interface;

namespace BackendProject.Controllers 
{
    [ApiController]
    [Route("visit")]
    [Route("visit/all")]
    public class VisitController : ControllerBase
    {
        public string Get()
        {
            using (var db = new DatabaseContext())
            {
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
        }
    }

    /*
    {
        "PatientId": ,
        "DoctorId": ,
        "ReceptionistId": 
    }
    */
    [ApiController]
    [Route("visit/register")]
    public class VisitRegisterController : ControllerBase
    {
        public IActionResult Post(PatientVisit input)
        {

            using (var db = new DatabaseContext())
            {

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
        }
    }

    /* 
    {
    "PatientVisitId": , 
    "Description": ""
    }
    */
    [ApiController]
    [Route("visit")]
    public class VisitCancelController : ControllerBase
    {
        [HttpPost("{visitId}/cancel")]
        public IActionResult Post(int visitId, SIVisitCancel formData)
        {
            using (var db = new DatabaseContext())
            {
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
        }
    }

    /* 
    {
    "PatientVisitId": , 
    "DoctorID": , 
    "Description": "",
    "Diagnosis": ""  
    }
    */
    [ApiController]
    [Route("visit")]
    public class VisitCloseController : ControllerBase
    {
        [HttpPost("{visitId}/close")]
        public IActionResult Post(PatientVisitForm input)
        {
            bool isInputValid = input.PatientVisitId != 0 && input.DoctorId != 0 && input.Description != null && input.Diagnosis != null;
            if (isInputValid)
            { // chyba musial wpisac opis, dowiemy sie
                // Biere login
                using (var db = new DatabaseContext())
                {
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
            }
            return BadRequest();
        }
    }

}