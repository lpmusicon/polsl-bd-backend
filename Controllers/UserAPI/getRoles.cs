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
    public class Role{
        public string mnemo { get; set; }
        public string name { get; set; }
    };
    
    [ApiController]
    [Route("api/user/getroles")]
    public class getRoleController : ControllerBase{
        public string Get(){
            
            List<Role> roles = new List<Role>();
            roles.Add(new Role {mnemo = "ADMN", name = "Administrator"});
            roles.Add(new Role {mnemo = "DOCT", name = "Lekarz"});
            roles.Add(new Role {mnemo = "RECP", name = "Recepcjonista"});
            roles.Add(new Role {mnemo = "LABW", name = "Pracownik Laboratorium"});
            roles.Add(new Role {mnemo = "LABM", name = "Kierownik Laboratorium"});
            return JsonSerializer.Serialize<List<Role>>(roles);
        }
    }
}
