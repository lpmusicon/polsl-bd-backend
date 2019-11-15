using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using bd_backend.Interface;

namespace bd_backend.Controllers{

    [ApiController]
    [Route("user/user_register")]
    /* DisabledTo nie trzeba wypelniac bedzie dzialac, PWZNumber potrzebny tylko do lekarza
    {
        "Login": "",
        "Password": "",
        "Role": "",
        "DisabledTo": "",
        "Name": "",
        "Lastname": "",
        "PWZNumber": ""
    }
    */
    public class UserRegisterController : ControllerBase{
        public IActionResult Post(RegisterData input){
            
            using (var db = new DatabaseContext()){
 
                if(db.Users.SingleOrDefault(x => x.Login == input.Login) != null) return BadRequest(); // sprawdzenie czy login istnieje

                if(input.Login != null && input.Password != null && input.Role != null && input.Name != null && input.Lastname != null 
                && (input.PWZNumber != null && input.PWZNumber.Length == 7 && input.PWZNumber.All(char.IsDigit) || (input.PWZNumber == null && input.Role != "DOCT"))){
                    var user = new User{
                        Login = input.Login,
                        Password = input.Password,
                        Role = input.Role,
                        DisabledTo = input.DisabledTo
                    };
                    if(input.Role != "RECP" && input.Role != "LABW" && input.Role != "LABM" && input.Role != "DOCT" && input.Role != "ADMN") return BadRequest(); // jesli zla rola
                    
                    db.Users.Add(user);
                    
                    db.SaveChanges(); // zapis usera do bazy
                    var login_record = db.Users.Single(x => x.Login == input.Login); // tutaj jest record w ktorym jest id tego loginu

                    switch(input.Role){
                        case "ADMN":
                            var ad = new Admin{
                                AdminId = login_record.UserId,
                                Name = input.Name,
                                Lastname = input.Lastname
                            };
                            db.Admins.Add(ad);
                        break;

                        case "RECP":
                            var rec = new Receptionist{
                                ReceptionistId = login_record.UserId,
                                Name = input.Name,
                                Lastname = input.Lastname
                            };
                            db.Receptionists.Add(rec);
                        break;

                        case "LABW":
                            var lw = new LaboratoryWorker{
                                LaboratoryWorkerId = login_record.UserId,
                                Name = input.Name,
                                Lastname = input.Lastname
                            };
                            db.LaboratoryWorkers.Add(lw);
                        break;

                        case "LABM":
                            var lm = new LaboratoryManager{
                                LaboratoryManagerId = login_record.UserId,
                                Name = input.Name,
                                Lastname = input.Lastname
                            };
                            db.LaboratoryManagers.Add(lm);
                        break;

                        case "DOCT":
                            var doc = new Doctor{
                                DoctorId = login_record.UserId,
                                Name = input.Name,
                                Lastname = input.Lastname,
                                PWZNumber = input.PWZNumber
                            };
                            db.Doctors.Add(doc);
                        break;
                    }
                    db.SaveChanges();
                    return StatusCode(201); // Created
                }
                return BadRequest();
            }
        }
    }

    [ApiController]
    [Route("user/change_disable_date")]
    /*
    {
	"Login": "",
	"NewDisableTime": ""
    }
    */
    public class ChangeDisableToController : ControllerBase{
        public IActionResult Post(ChangeDisableDate ndt){
            if(ndt.Login != null && ndt.NewDisableTime != null){
                // Biere login
                using (var db = new DatabaseContext()){
                    var user = db.Users.SingleOrDefault(x => x.Login == ndt.Login); // tutaj sprawdzanie czy login jest unikalny // JAK ZROBIC UNIQUE W TABELACH
                    if(user != null){
                        user.DisabledTo = ndt.NewDisableTime;
                        db.SaveChanges();
                        return Ok();
                    }else return NotFound();
                } 
            }else return BadRequest();
        }
    }

    [ApiController]
    [Route("user/change_password")]
    /*
    {
	"Login": "",
	"NewPassword": ""
    }
    */
    public class ChangePasswordController : ControllerBase{
        public IActionResult Post(ChangePassword input){
            
            if(input.Login != null && input.NewPassword != null){
                // Biere login
                using (var db = new DatabaseContext()){
                    var user = db.Users.SingleOrDefault(x => x.Login == input.Login); 
                    if(user != null){
                        user.Password = input.NewPassword;
                        db.SaveChanges();
                        return Ok();
                    }else return NotFound();
                } 
            }else return BadRequest();
        }
    }

    [ApiController]
    [Route("user/get_users")]
    public class GetUsersController : ControllerBase{
        public string Get(){
            
            var result = (from x in new DatabaseContext().Users 
            select new GetUser {UserId = x.UserId, Login = x.Login, Role = x.Role, DisabledTo = x.DisabledTo }).ToList();  

            return JsonSerializer.Serialize<List<GetUser>>(result);
        }
    }
    
    [ApiController]
    [Route("user/get_roles")]
    public class GetRoleController : ControllerBase{
        public string Get(){
            
            List<Role> roles = new List<Role>();
            roles.Add(new Role {Mnemo = "ADMN", Name = "Administrator"});
            roles.Add(new Role {Mnemo = "DOCT", Name = "Lekarz"});
            roles.Add(new Role {Mnemo = "RECP", Name = "Recepcjonista"});
            roles.Add(new Role {Mnemo = "LABW", Name = "Pracownik Laboratorium"});
            roles.Add(new Role {Mnemo = "LABM", Name = "Kierownik Laboratorium"});

            return JsonSerializer.Serialize<List<Role>>(roles);
        }
    }
}
