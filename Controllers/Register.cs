using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization; 

namespace bd_backend.Controllers
{
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
    [Route("api/register")]
    public class RegisterController : ControllerBase{
        public string Post(RegisterData nu)
        {
            using (var db = new DatabaseContext()){
                var uniq = db.Users.SingleOrDefault(x => x.Login == nu.Login); // tutaj sprawdzanie czy login jest unikalny // JAK ZROBIC UNIQUE W TABELACH
                if(uniq != null) return "lol";
                if(nu.Login != null && nu.Password != null && nu.Role != null && nu.Name != null && nu.Lastname != null && (nu.PWZNumber != null || (nu.PWZNumber == null && nu.Role != "Doctor"))){
                    var user = new User{
                        Login = nu.Login,
                        Password = nu.Password,
                        Role = nu.Role,
                        DisabledTo = nu.DisabledTo
                    };
                    if(nu.Role != "Recepcionist" && nu.Role != "LaboratoryWorker" && nu.Role != "LaboratoryManager" && nu.Role != "Doctor") return "Bad profession"; // jesli zla rola
                    
                    db.Users.Add(user);
                    
                    db.SaveChanges(); // zapis usera do bazy
                    var ulr = db.Users.Single(x => x.Login == nu.Login); // tutaj jest record w ktorym jest id tego loginu

                    if(nu.Role == "Recepcionist"){
                        var profession = new Receptionist{
                            ReceptionistId = ulr.UserId,
                            Name = nu.Name,
                            Lastname = nu.Lastname
                        };
                        db.Receptionists.Add(profession);
                    }else if(nu.Role == "LaboratoryWorker"){
                        var profession = new LaboratoryWorker{
                            LaboratoryWorkerId = ulr.UserId,
                            Name = nu.Name,
                            Lastname = nu.Lastname
                        };
                        db.LaboratoryWorkers.Add(profession);
                    }else if(nu.Role == "LaboratoryManager"){
                        var profession = new LaboratoryManager{
                            LaboratoryManagerId = ulr.UserId,
                            Name = nu.Name,
                            Lastname = nu.Lastname
                        };
                        db.LaboratoryManagers.Add(profession);
                    }else if(nu.Role == "Doctor"){
                        var profession = new Doctor{
                            DoctorId = ulr.UserId,
                            Name = nu.Name,
                            Lastname = nu.Lastname,
                            PWZNumber = nu.PWZNumber
                        };
                        db.Doctors.Add(profession);
                    }
                    db.SaveChanges();
                    return "Success";
                }
                return "Error, submited nulls";
            }
        }
    }
}
