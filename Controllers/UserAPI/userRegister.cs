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
    public class RegisterData{
        public string Login { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime DisabledTo { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string PWZNumber { get; set; }
    };
    [ApiController]
    [Route("api/user/register")]
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
    public class userRegisterController : ControllerBase{
        public IActionResult Post(RegisterData nu){
            
            using (var db = new DatabaseContext()){
                var uniq = db.Users.SingleOrDefault(x => x.Login == nu.Login); // tutaj sprawdzanie czy login jest unikalny // JAK ZROBIC UNIQUE W TABELACH
                if(uniq != null) return BadRequest();
                if(nu.Login != null && nu.Password != null && nu.Role != null && nu.Name != null && nu.Lastname != null && (nu.PWZNumber != null && nu.PWZNumber.Length == 7 && nu.PWZNumber.All(char.IsDigit) || (nu.PWZNumber == null && nu.Role != "DOCT"))){
                    var user = new User{
                        Login = nu.Login,
                        Password = nu.Password,
                        Role = nu.Role,
                        DisabledTo = nu.DisabledTo
                    };
                    if(nu.Role != "RECP" && nu.Role != "LABW" && nu.Role != "LABM" && nu.Role != "DOCT" && nu.Role != "ADMN") return BadRequest(); // jesli zla rola
                    
                    db.Users.Add(user);
                    
                    db.SaveChanges(); // zapis usera do bazy
                    var ulr = db.Users.Single(x => x.Login == nu.Login); // tutaj jest record w ktorym jest id tego loginu

                    switch(nu.Role){
                        case "ADMN":
                            var ad = new Admin{
                                AdminId = ulr.UserId,
                                Name = nu.Name,
                                Lastname = nu.Lastname
                            };
                            db.Admins.Add(ad);
                        break;

                        case "RECP":
                            var rec = new Receptionist{
                                ReceptionistId = ulr.UserId,
                                Name = nu.Name,
                                Lastname = nu.Lastname
                            };
                            db.Receptionists.Add(rec);
                        break;

                        case "LABW":
                            var lw = new LaboratoryWorker{
                                LaboratoryWorkerId = ulr.UserId,
                                Name = nu.Name,
                                Lastname = nu.Lastname
                            };
                            db.LaboratoryWorkers.Add(lw);
                        break;

                        case "LABM":
                            var lm = new LaboratoryManager{
                                LaboratoryManagerId = ulr.UserId,
                                Name = nu.Name,
                                Lastname = nu.Lastname
                            };
                            db.LaboratoryManagers.Add(lm);
                        break;

                        case "DOCT":
                            var doc = new Doctor{
                                DoctorId = ulr.UserId,
                                Name = nu.Name,
                                Lastname = nu.Lastname,
                                PWZNumber = nu.PWZNumber
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
}
