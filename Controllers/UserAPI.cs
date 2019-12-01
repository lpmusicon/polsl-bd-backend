using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using BackendProject.Models;
using BackendProject.Helpers;
using System;

namespace BackendProject.Controllers
{
    [ApiController]
    [Route("User")]
    public class UserController: ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }
        
        /* PWZNumber potrzebny tylko do lekarza
        {
            "Login": "",
            "Password": "",
            "Role": "",
            "ExpiryDate": "",
            "Name": "",
            "Lastname": "",
            "PWZNumber": ""
        }
        */
        [HttpPost("register")]
        [Authorize(Roles="ADMN")]
        public IActionResult Register(RegisterModel input)
        {
            using var db = new DatabaseContext();
            // sprawdzenie czy istnieje
            if (db.Users.SingleOrDefault(x => x.Login == input.Login) != null)
                return BadRequest();

            if (input.Login != null &&
                input.Password != null &&
                input.Name != null &&
                input.Lastname != null &&
                input.ExpiryDate > DateTime.Now &&
                (input.Role == "RECP" || // sprawdzanie rol
                input.Role == "LABW" ||
                input.Role == "LABM" || 
                input.Role == "DOCT" || 
                input.Role == "ADMN") &&
                (input.PWZNumber != null && input.PWZNumber.Length == 7 && input.PWZNumber.All(char.IsDigit) || // jesli rejstracja doktora konieczny sprecyzjowany PWZ
                ((input.PWZNumber == null || input.PWZNumber == "") && input.Role != "DOCT"))) // jesli rejstracja kogos innego niz doktor nie ma PZW
            {

                db.Users.Add(new User{
                    Login = input.Login,
                    Password = new PasswordHelper().CreateHashedPassword(input.Password), // tworzenie zahashowanego hasla
                    Role = input.Role,
                    ExpiryDate = input.ExpiryDate,
                    NeverExpires = false
                });
                db.SaveChanges(); // zapis usera do bazy
                var login_record = db.Users.Single(x => x.Login == input.Login); // tutaj jest record w ktorym jest id tego loginu

                switch (input.Role)
                {
                    case "ADMN":
                        db.Admins.Add(new Admin(){
                            AdminId = login_record.UserId,
                            Name = input.Name,
                            Lastname = input.Lastname
                        });
                        break;

                    case "RECP":
                        db.Receptionists.Add(new Receptionist(){
                            ReceptionistId = login_record.UserId,
                            Name = input.Name,
                            Lastname = input.Lastname
                        });
                        break;

                    case "LABW":
                        db.LaboratoryWorkers.Add(new LaboratoryWorker(){
                            LaboratoryWorkerId = login_record.UserId,
                            Name = input.Name,
                            Lastname = input.Lastname
                        });
                        break;

                    case "LABM":
                        db.LaboratoryManagers.Add(new LaboratoryManager(){
                            LaboratoryManagerId = login_record.UserId,
                            Name = input.Name,
                            Lastname = input.Lastname
                        });
                        break;

                    case "DOCT":
                        db.Doctors.Add(new Doctor(){
                            DoctorId = login_record.UserId,
                            Name = input.Name,
                            Lastname = input.Lastname,
                            PWZNumber = input.PWZNumber
                        });
                        break;
                }
                db.SaveChanges();
                return NoContent(); // Created
            }
            return BadRequest("Bad luck");
        }
    
        /*
        {
            "Login": "",
            "NewDisableTime": ""
        }
        */
        [HttpPatch("{userId}/disable")]
        [Authorize(Roles="ADMN")]
        public IActionResult Disable(int userId, DisableDateModel input)
        {
            using var db = new DatabaseContext();
            var user = db.Users.SingleOrDefault(x => x.UserId == userId);
            if (user != null)
            {
                user.ExpiryDate = input.ExpiryDate;
                user.NeverExpires = input.NeverExpires;
                db.SaveChanges();
                return NoContent();
            }
            return BadRequest();
        }

        /*
        {
            "Login": "",
            "NewPassword": ""
        }
        */
        [HttpPatch("{userId}/passwd")]
        [Authorize(Roles="ADMN")]
        public IActionResult Password(int userId, PasswordModel input)
        {
            using var db = new DatabaseContext();
            var user = db.Users.SingleOrDefault(x => x.UserId == userId);
            
            if (user != null && input.NewPassword != null)
            {
                var pw = new PasswordHelper();
                string newPassword = pw.CreateHashedPassword(input.NewPassword);
                user.Password = newPassword;
                db.SaveChanges();
                return NoContent();
            }
            return BadRequest();
        }

        [HttpGet]
        [HttpGet("all")]
        [Authorize(Roles="ADMN")]
        public List<UserModel> All()
        {
            using var db = new DatabaseContext();

            List<UserModel> UsersList = new List<UserModel>();
            var recp = (from u in db.Users
                          join r in db.Receptionists on u.UserId equals r.ReceptionistId
                          select new UserModel
                          {
                              UserId = u.UserId,
                              Login = u.Login,
                              Name = r.Name,
                              Lastname = r.Lastname,
                              Role = u.Role,
                              ExpiryDate = u.ExpiryDate,
                              NeverExpires = u.NeverExpires
                          }).ToList(); // lista wszystkich recepcjonistow
            foreach(UserModel item in recp) UsersList.Add(item); // wpisywanie wszystkich recepcjonistow do listy userow

            var doct = (from u in db.Users
                          join d in db.Doctors on u.UserId equals d.DoctorId
                          select new UserModel
                          {
                              UserId = u.UserId,
                              Login = u.Login,
                              Name = d.Name,
                              Lastname = d.Lastname,
                              Role = u.Role,
                              ExpiryDate = u.ExpiryDate,
                              NeverExpires = u.NeverExpires
                          }).ToList(); // lista wszystkich lekarzy
            foreach(UserModel item in doct) UsersList.Add(item); // wpisywanie wszystkich lekarzy do listy userow

            var labw = (from u in db.Users
                          join lw in db.LaboratoryWorkers on u.UserId equals lw.LaboratoryWorkerId
                          select new UserModel
                          {
                              UserId = u.UserId,
                              Login = u.Login,
                              Name = lw.Name,
                              Lastname = lw.Lastname,
                              Role = u.Role,
                              ExpiryDate = u.ExpiryDate,
                              NeverExpires = u.NeverExpires
                          }).ToList(); // lista wszystkich laborantow
            foreach(UserModel item in labw) UsersList.Add(item); // wpisywanie wszystkich laborantow do listy userow

            var labm = (from u in db.Users
                          join lm in db.LaboratoryManagers on u.UserId equals lm.LaboratoryManagerId
                          select new UserModel
                          {
                              UserId = u.UserId,
                              Login = u.Login,
                              Name = lm.Name,
                              Lastname = lm.Lastname,
                              Role = u.Role,
                              ExpiryDate = u.ExpiryDate,
                              NeverExpires = u.NeverExpires
                          }).ToList(); // lista wszystkich managerow
            foreach(UserModel item in labm) UsersList.Add(item); // wpisywanie wszystkich managerow do listy userow

            var admn = (from u in db.Users
                          join a in db.Admins on u.UserId equals a.AdminId
                          select new UserModel
                          {
                              UserId = u.UserId,
                              Login = u.Login,
                              Name = a.Name,
                              Lastname = a.Lastname,
                              Role = u.Role,
                              ExpiryDate = u.ExpiryDate,
                              NeverExpires = u.NeverExpires
                          }).ToList(); // lista wszystkich adminow
            foreach(UserModel item in admn) UsersList.Add(item); // wpisywanie wszystkich adminow do listy userow
            
            return UsersList;
        }

        [HttpGet("roles")]
        [Authorize(Roles="ADMN")]
        public List<RoleModel> Roles()
        {
            List<RoleModel> roles = new List<RoleModel>() {
                new RoleModel { Mnemo = "ADMN", Name = "Administrator" },
                new RoleModel { Mnemo = "DOCT", Name = "Lekarz" },
                new RoleModel { Mnemo = "RECP", Name = "Recepcjonista" },
                new RoleModel { Mnemo = "LABW", Name = "Pracownik Laboratorium" },
                new RoleModel { Mnemo = "LABM", Name = "Kierownik Laboratorium" }
            };

            return roles;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(AuthenticateModel model)
        {
            var user = await _userService.Authenticate(model.Login, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });
	
            if(user.ExpiryDate < DateTime.Now && !user.NeverExpires)
                return BadRequest(new { message = "User is disabled" });
               
            return Ok(user);
        }
    }
}