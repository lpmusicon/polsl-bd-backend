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
    [Route("api/recepcion/patientregister")]
    public class patientRegisterController : ControllerBase{
        public IActionResult Post(Patient np){
            
            using (var db = new DatabaseContext()){

                var uniq = db.Patients.SingleOrDefault(x => x.PESEL == np.PESEL); // tutaj sprawdzanie czy login jest unikalny // JAK ZROBIC UNIQUE W TABELACH
                if(uniq != null) return BadRequest();

                if(np.PatientId == 0 && np.PESEL.Length == 11 && np.PESEL.All(char.IsDigit)){
                    db.Patients.Add(np);
                    db.SaveChanges();
                    return StatusCode(201);
                }else return BadRequest();
            }
        }
    }
}
