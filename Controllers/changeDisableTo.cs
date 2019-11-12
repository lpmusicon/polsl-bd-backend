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
    public class ChangeDT{
        public string Login { get; set; }
        public DateTime newDisableTime { get; set; }
    };
    [ApiController]
    [Route("api/changedisabletime")]
    public class changeDisableToController : ControllerBase{
        public IActionResult Post(ChangeDT ndt){
            if(ndt.Login != null && ndt.newDisableTime != null){
                // Biere login
                using (var db = new DatabaseContext()){
                    var user = db.Users.SingleOrDefault(x => x.Login == ndt.Login); // tutaj sprawdzanie czy login jest unikalny // JAK ZROBIC UNIQUE W TABELACH
                    if(user != null){
                        user.DisabledTo = ndt.newDisableTime;
                        db.SaveChanges();
                        return Ok();
                    }else return NotFound();
                } 
            }else return BadRequest();
        }
    }
}
