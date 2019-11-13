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
    public class DPV{
        public string patientName { get; set; }
        public string patientLastname { get; set; }
        public DateTime registerDate { get; set; }

    };
    public class ADPV: DPV{
        public string doctorName { get; set; }
        public string doctorLastname { get; set; }

    };
    [ApiController]
    [Route("api/doctor/getpatientvisits/")]
    public class getDoctorsPatientVisitsController : ControllerBase{
        [HttpGet("{id}")]
        public string Get(int id){
            using (var db = new DatabaseContext()){
            var result = (from p in db.Patients join pv in db.PatientVisits on p.PatientId equals pv.PatientId
            where pv.DoctorId == id && pv.Status == "Registered" select  new DPV { patientName = p.Name, patientLastname = p.Lastname, registerDate = pv.RegisterDate }).ToList();  
            return JsonSerializer.Serialize<System.Collections.Generic.List<bd_backend.Controllers.DPV>>(result);
            }
        }
    public string Get(){
            using (var db = new DatabaseContext()){
            var result = (from p in db.Patients join pv in db.PatientVisits on p.PatientId equals pv.PatientId
            join d in db.Doctors on pv.DoctorId equals d.DoctorId
            where pv.Status == "Registered" select  new ADPV { doctorName = d.Name, doctorLastname = d.Lastname, patientName = p.Name, 
            patientLastname = p.Lastname, registerDate = pv.RegisterDate }).ToList();  
            return JsonSerializer.Serialize<System.Collections.Generic.List<bd_backend.Controllers.ADPV>>(result);
            }
        }
    }
}