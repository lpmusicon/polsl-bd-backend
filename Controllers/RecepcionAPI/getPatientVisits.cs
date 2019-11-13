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
    [Route("api/recepcion/getpatientvisits")]
    public class getPatientVisitsController : ControllerBase{
        public string Get(){
             using (var db = new DatabaseContext()){
                 var pv = db.PatientVisits;
            
            return JsonSerializer.Serialize<Microsoft.EntityFrameworkCore.DbSet<PatientVisit>>(pv);
             }
        }
    }
}
