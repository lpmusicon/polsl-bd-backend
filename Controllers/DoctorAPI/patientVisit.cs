using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization; 

namespace bd_backend.Controllers{
    public class PVF{
        public int patientVisitId { get; set; }
        public int doctorId { get; set; } // to dlatego ze mozna podmieniac doktorow w wizycie, rowniez do sprawdzenia, narazie samowolka jak w latach 90-tych
        public string description { get; set; }
        public string diagnosis { get; set; }
    };
    [ApiController]
    [Route("api/doctor/patientvisit")]
    /* 
    {
    "patientVisitId": , 
    "doctorID": , 
    "description": "",
    "diagnosis": ""  
    }
    */
    public class patientVisitController : ControllerBase{
        public IActionResult Post(PVF npv){
            
            if(npv.patientVisitId != 0 && npv.doctorId != 0 && npv.description != null){ // chyba musial wpisac opis, dowiemy sie
                // Biere login
                using (var db = new DatabaseContext()){
                    var pv = db.PatientVisits.SingleOrDefault(x => x.PatientVisitId == npv.patientVisitId); 
                    if(pv != null && pv.Status == "Registered" && db.Doctors.SingleOrDefault(x => x.DoctorId == npv.doctorId) != null ){
                        pv.Diagnosis = npv.diagnosis;
                        pv.Description = npv.description;
                        pv.DoctorId = npv.doctorId; 
                        pv.CloseDate = DateTime.Now;
                        pv.Status = "Closed";
                        db.SaveChanges();
                        return Ok();
                    }else return NotFound();
                } 
            }else return BadRequest();
        }
    }
}
