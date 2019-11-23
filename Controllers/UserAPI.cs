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
        [HttpPost("register")]
        [Authorize(Roles="ADMN")]
        public IActionResult Register([FromForm]RegisterData input)
        {
            using var db = new DatabaseContext();
            // sprawdzenie czy istnieje
            if (db.Users.SingleOrDefault(x => x.Login == input.Login) != null)
                return BadRequest();

            if (input.Login != null &&
                input.Password != null &&
                input.Role != null &&
                input.Name != null &&
                input.Lastname != null &&
                (input.PWZNumber != null && input.PWZNumber.Length == 7 && input.PWZNumber.All(char.IsDigit) ||
                (input.PWZNumber == null && input.Role != "DOCT")))
            {

                var pw = new PasswordHelper();
                var user = new User
                {
                    Login = input.Login,
                    Password = pw.CreateHashedPassword(input.Password),
                    Role = input.Role,
                    DisabledTo = input.DisabledTo
                };

                if (input.Role != "RECP" && input.Role != "LABW" && input.Role != "LABM" && input.Role != "DOCT" && input.Role != "ADMN")
                    return BadRequest(); // jesli zla rola

                db.Users.Add(user);
                db.SaveChanges(); // zapis usera do bazy
                var login_record = db.Users.Single(x => x.Login == input.Login); // tutaj jest record w ktorym jest id tego loginu

                switch (input.Role)
                {
                    case "ADMN":
                        Admin ad = new Admin
                        {
                            AdminId = login_record.UserId,
                            Name = input.Name,
                            Lastname = input.Lastname
                        };
                        db.Admins.Add(ad);
                        break;

                    case "RECP":
                        Receptionist rec = new Receptionist
                        {
                            ReceptionistId = login_record.UserId,
                            Name = input.Name,
                            Lastname = input.Lastname
                        };
                        db.Receptionists.Add(rec);
                        break;

                    case "LABW":
                        LaboratoryWorker lw = new LaboratoryWorker
                        {
                            LaboratoryWorkerId = login_record.UserId,
                            Name = input.Name,
                            Lastname = input.Lastname
                        };
                        db.LaboratoryWorkers.Add(lw);
                        break;

                    case "LABM":
                        LaboratoryManager lm = new LaboratoryManager
                        {
                            LaboratoryManagerId = login_record.UserId,
                            Name = input.Name,
                            Lastname = input.Lastname
                        };
                        db.LaboratoryManagers.Add(lm);
                        break;

                    case "DOCT":
                        Doctor doc = new Doctor
                        {
                            DoctorId = login_record.UserId,
                            Name = input.Name,
                            Lastname = input.Lastname,
                            PWZNumber = input.PWZNumber
                        };
                        db.Doctors.Add(doc);
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
        [HttpPost("{userId}/disable")]
        [Authorize(Roles="ADMN")]
        public IActionResult Disable(int userId, DisableDateModel ndt)
        {
            if (userId != 0 && ndt.DisableTime != null)
            {
                // Biere login
                using var db = new DatabaseContext();
                var user = db.Users.SingleOrDefault(x => x.UserId == userId);
                if (user != null)
                {
                    user.DisabledTo = ndt.DisableTime;
                    db.SaveChanges();
                    return NoContent();
                }
                return NotFound();
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
        public IActionResult Password(ChangePassword input)
        {

            if (input.Login != null && input.NewPassword != null)
            {
                // Biere login
                using var db = new DatabaseContext();
                var user = db.Users.SingleOrDefault(x => x.Login == input.Login);
                if (user != null)
                {
                    var pw = new PasswordHelper();
                    string newPassword = pw.CreateHashedPassword(input.NewPassword);

                    user.Password = newPassword;
                    db.SaveChanges();
                    return NoContent();
                }
                return NotFound();
            }
            return BadRequest();
        }

        [HttpGet]
        [HttpGet("all")]
        [Authorize(Roles="ADMN")]
        public List<GetUser> All()
        {
            using var db = new DatabaseContext();

            List<GetUser> UsersList = new List<GetUser>();
            var recp = (from u in db.Users
                          join r in db.Receptionists on u.UserId equals r.ReceptionistId
                          select new GetUser
                          {
                              UserId = u.UserId,
                              Login = u.Login,
                              Name = r.Name,
                              Lastname = r.Lastname,
                              Role = u.Role,
                              DisabledTo = u.DisabledTo
                          }).ToList();
            foreach(GetUser item in recp) UsersList.Add(item);

            var doct = (from u in db.Users
                          join d in db.Doctors on u.UserId equals d.DoctorId
                          select new GetUser
                          {
                              UserId = u.UserId,
                              Login = u.Login,
                              Name = d.Name,
                              Lastname = d.Lastname,
                              Role = u.Role,
                              DisabledTo = u.DisabledTo
                          }).ToList();
            foreach(GetUser item in doct) UsersList.Add(item);

            var labw = (from u in db.Users
                          join lw in db.LaboratoryWorkers on u.UserId equals lw.LaboratoryWorkerId
                          select new GetUser
                          {
                              UserId = u.UserId,
                              Login = u.Login,
                              Name = lw.Name,
                              Lastname = lw.Lastname,
                              Role = u.Role,
                              DisabledTo = u.DisabledTo
                          }).ToList();
            foreach(GetUser item in labw) UsersList.Add(item);

            var labm = (from u in db.Users
                          join lm in db.LaboratoryManagers on u.UserId equals lm.LaboratoryManagerId
                          select new GetUser
                          {
                              UserId = u.UserId,
                              Login = u.Login,
                              Name = lm.Name,
                              Lastname = lm.Lastname,
                              Role = u.Role,
                              DisabledTo = u.DisabledTo
                          }).ToList();
            foreach(GetUser item in labm) UsersList.Add(item);

            var admn = (from u in db.Users
                          join a in db.Admins on u.UserId equals a.AdminId
                          select new GetUser
                          {
                              UserId = u.UserId,
                              Login = u.Login,
                              Name = a.Name,
                              Lastname = a.Lastname,
                              Role = u.Role,
                              DisabledTo = u.DisabledTo
                          }).ToList();
            foreach(GetUser item in admn) UsersList.Add(item);
            
            return UsersList;
        }

        [HttpGet("roles")]
        [Authorize(Roles="ADMN")]
        public List<RoleModel> Get()
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
	
            if(user.DisabledTo > DateTime.Now)
                return BadRequest(new { message = "User is disabled" });
               
            return Ok(user);
        }
    }
}