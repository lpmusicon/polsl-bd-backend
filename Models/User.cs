using System;
using System.ComponentModel.DataAnnotations;
namespace BackendProject.Models
{
    public class AuthenticateModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class RoleModel
    {
        public string Mnemo { get; set; }
        public string Name { get; set; }
    };

    public class UserModel
    {
        public int UserId { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Role { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool NeverExpires { get; set; }
    };

    public class PasswordModel
    {
        public string NewPassword { get; set; }
    };

    public class DisableDateModel
    {
        public DateTime ExpiryDate { get; set; }
        public bool NeverExpires { get; set; }
    };

    public class RegisterModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
        public DateTime ExpiryDate { get; set; } // to mozna zostawic puste, wtedy data bedzie najwczesniejsza i konto bedzie domyslnie aktywne

        [Required]
        public string Name { get; set; }

        [Required]
        public string Lastname { get; set; }

        public string PWZNumber { get; set; }
    };
}