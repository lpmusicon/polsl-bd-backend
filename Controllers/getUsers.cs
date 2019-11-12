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
    [Route("api/getusers")]
    public class getUsersController : ControllerBase{
        public string Get(){
             using (var db = new DatabaseContext()){
                 var users = db.Users;
            List<Role> roles = new List<Role>();
            
            return JsonSerializer.Serialize<Microsoft.EntityFrameworkCore.DbSet<User>>(users);
             }
        }
    }
}
