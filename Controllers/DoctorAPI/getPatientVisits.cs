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
    [ApiController]
    [Route("api/doctor/getpatientvisits/")]
    public class getDoctorsPatientVisitsController : ControllerBase{
        [HttpGet("{id}")]
        public int Get(int id){
            return id;
             //using (var db = new DatabaseContext()){
              //   var pv = db.PatientVisits.Select(x=> x.DoctorId == DoctorId);
            
            //return JsonSerializer.Serialize<Microsoft.EntityFrameworkCore.DbSet<PatientVisit>>(pv);
             //}
        }
    }
}
