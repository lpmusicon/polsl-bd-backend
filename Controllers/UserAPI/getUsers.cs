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
    public class userGetter{
        public int UserId { get; set; }
        public string Login { get; set; }
        public string Role { get; set; }
        public DateTime DisabledTo { get; set; }
    };

    [ApiController]
    [Route("api/user/getusers")]
    public class getUsersController : ControllerBase{
        public string Get(){
            
            var result = (from x in new DatabaseContext().Users select new userGetter {UserId = x.UserId, Login = x.Login, Role = x.Role, DisabledTo = x.DisabledTo }).ToList();  
            return JsonSerializer.Serialize<System.Collections.Generic.List<bd_backend.Controllers.userGetter>>(result);
        }
    }
}
