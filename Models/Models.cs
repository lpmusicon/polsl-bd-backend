using System;
using System.ComponentModel.DataAnnotations;

namespace BackendProject.Models
{
    // User Interfaces
    public class RegisterModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
        public DateTime DisabledTo { get; set; } // to mozna zostawic puste, wtedy data bedzie najwczesniejsza i konto bedzie domyslnie aktywne

        [Required]
        public string Name { get; set; }

        [Required]
        public string Lastname { get; set; }

        public string PWZNumber { get; set; }
    };

    public class DisableDateModel
    {
        public DateTime DisableTime { get; set; }
    };

    public class PasswordModel
    {
        public string NewPassword { get; set; }
    };

    public class UserModel
    {
        public int UserId { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Role { get; set; }
        public DateTime DisabledTo { get; set; }
    };

    public class RoleModel
    {
        public string Mnemo { get; set; }
        public string Name { get; set; }
    };

    // LaboratoryWorker Interfaces
    public class ResultModel
    {
        public string Result { get; set; }
    };

    public struct ReasonModel
    {
        public string Reason { get; set; }
    }

    // Doctor Interfaces
    public class PatientVisitModel
    {
        public string Description { get; set; }
        public string Diagnosis { get; set; }
    };

    public class PatientsVisitsModel
    {
        public int Id { get; set; }
        public PatientModel Patient { get; set; }
        public DateTime RegisterDate { get; set; }

    };
    public class AllPatientsVisitsModel : PatientsVisitsModel
    {
        public DoctorModel Doctor { get; set; }
    };

    public class PatientVisitsModel : AllPatientsVisitsModel
    {
        public int PatientVisitId { get; set; }
        public string Description { get; set; }
        public string Diagnosis { get; set; }
        public string Status { get; set; }
        public DateTime? CloseDate { get; set; }
    };

    public class AuthenticateModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class PatientLaboratoryExaminationsModel{
        public string ExaminationName { get; set; }
        public string Result { get; set; }
        public string DoctorName { get; set; }
        public string DoctorLastName { get; set; }
        public DateTime OrderExaminationDate { get; set; }
        public DateTime? ExecuteExaminationDate { get; set; }
    }

    public class PatientPhysicalExaminationsModel{
        public string ExaminationName { get; set; }
        public string Result { get; set; }
        public string DoctorName { get; set; }
        public string DoctorLastName { get; set; }
        public DateTime? ExaminationDate { get; set; }
    }
}