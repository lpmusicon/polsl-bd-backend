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
    public class ChangePassword{
        public string Login { get; set; }
        public string newPassword { get; set; }
    };
    [ApiController]
    [Route("api/user/changepassword")]
    public class changePasswordController : ControllerBase{
        public IActionResult Post(ChangePassword np){
            
            if(np.Login != null && np.newPassword != null){
                // Biere login
                using (var db = new DatabaseContext()){
                    var user = db.Users.SingleOrDefault(x => x.Login == np.Login); 
                    if(user != null){
                        user.Password = np.newPassword;
                        db.SaveChanges();
                        return Ok();
                    }else return NotFound();
                } 
            }else return BadRequest();
        }
    }
}
