using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization; 
using Microsoft.EntityFrameworkCore;
using bd_backend.Interface;

namespace bd_backend.Controllers{ 

    [ApiController]
    [Route("recepcion/patient_register")]
    /*
    {
    "Name": "",
    "Lastname": "",
    "PESEL": ""
    }

    */
    public class PatientRegisterController : ControllerBase{
        public IActionResult Post(Patient input){
            
            using (var db = new DatabaseContext()){

                if(db.Patients.SingleOrDefault(x => x.PESEL == input.PESEL) != null) return BadRequest(); // tutaj sprawdzanie czy login jest unikalny

                if(input.PatientId == 0 && input.PESEL.Length == 11 && input.PESEL.All(char.IsDigit)){
                    db.Patients.Add(input);
                    db.SaveChanges();
                    return StatusCode(201);
                }else return BadRequest();
            }
        }
    }

    [ApiController]
    [Route("recepcion/patient_visit_register")]
    /*
    {
    "PatientId": ,
	"DoctorId": ,
	"ReceptionistId": 
    }
    */
    public class PatientVisitRegisterController : ControllerBase{
        public IActionResult Post(PatientVisit input){
            
            using (var db = new DatabaseContext()){

                if(db.Patients.SingleOrDefault(x => x.PatientId == input.PatientId) == null) return BadRequest();

                if(input.PatientVisitId == 0 && input.Description == null && input.Diagnosis == null && input.CloseDate == null){
                    input.Status = "Registered";
                    input.RegisterDate = DateTime.Now;
                    db.PatientVisits.Add(input);
                    db.SaveChanges();
                    return StatusCode(201);
                }else return BadRequest();
            }
        }
    }

    [ApiController]
    [Route("recepcion/patient_visit_cancel")]
    /* 
    {
    "PatientVisitId": , 
    "Description": ""
    }
    */
    public class PatientVisitCancelController : ControllerBase{
        public IActionResult Post(PatientVisitCancel input){
            using (var db = new DatabaseContext()){
                
                var pv = db.PatientVisits.SingleOrDefault(x => x.PatientVisitId == input.PatientVisitId);
                if(pv != null){
                    pv.Status = "Canceled";
                    pv.CloseDate = DateTime.Now;
                    pv.Description = input.Description;
                    db.SaveChanges();
                    return Ok();
                }return BadRequest();
            }
        }
    }

    [ApiController]
    [Route("recepcion/get_patients")]
    [Route("doctor/get_patients")]

    public class GetPatients : ControllerBase{
        public string Get(){
            using (var db = new DatabaseContext()){
                return JsonSerializer.Serialize<DbSet<Patient>>(db.Patients);
            }
        }
    }

    [ApiController]
    [Route("recepcion/get_doctors")]
    public class GetDoctorsController : ControllerBase{
        public string Get(){
            using (var db = new DatabaseContext()){
                var result = (from x in db.Doctors select new DoctorsList { DoctorId = x.DoctorId, Name = x.Name, Lastname = x.Lastname }).ToList();

                return JsonSerializer.Serialize<List<DoctorsList>>(result);
             }
        }
    }
}
