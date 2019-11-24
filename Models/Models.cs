using System;
using System.ComponentModel.DataAnnotations;

namespace BackendProject.Models
{
    // User Interfaces
    public class RegisterData
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

    public class ChangePassword
    {
        public string Login { get; set; }
        public string NewPassword { get; set; }
    };

    public class GetUser
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

    public class DoctorsList
    {
        public int DoctorId { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
    };
    // LaboratoryWorker Interfaces
    public class ExaminationExecute
    {
        public string Result { get; set; }
        public int LaboratoryWorkerId { get; set; }
    };

    public class ExaminationCancel
    {
        public int LaboratoryWorkerId { get; set; }
    };

    public class OrderedExaminationList
    {
        public string DoctorComment { get; set; }
        public string LaboratoryExaminatonName { get; set; }
        public DateTime OrderDate { get; set; }
    };
    // LaboratoryManager Interfaces
    public class ExaminationApproval
    {
        public int LaboratoryManagerId { get; set; }
    };

    public class ExaminationReject
    {
        public int LaboratoryManagerId { get; set; }
        public string ManagerComment { get; set; }
    };

    public class ExecutedExaminationList
    {
        public string Result { get; set; }
        public string DoctorComment { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ExaminationDate { get; set; }
        public string Status { get; set; }
        public string LabWorkerName { get; set; }
        public string LabWorkerLastname { get; set; }
    };

    public class ResolvedExaminationList : ExecutedExaminationList
    {
        public string LabManagerName { get; set; }
        public string LabManagerLastname { get; set; }
        public DateTime? ApprovalRejectionDate { get; set; }
    };

    // Doctor Interfaces
    public class PatientVisitForm
    {
        public int PatientVisitId { get; set; }
        public int DoctorId { get; set; } // to dlatego ze mozna podmieniac doktorow w wizycie, rowniez do sprawdzenia, narazie samowolka jak w latach 90-tych
        public string Description { get; set; }
        public string Diagnosis { get; set; }
    };

    public class PatientsVisitsList
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientLastname { get; set; }
        public DateTime RegisterDate { get; set; }

    };
    public class AllPatientsVisitsList : PatientsVisitsList
    {
        public string DoctorName { get; set; }
        public string DoctorLastname { get; set; }

    };

    public class PatientVisitsList : AllPatientsVisitsList
    {
        public int PatientVisitId { get; set; }
        public string Description { get; set; }
        public string Diagnosis { get; set; }
        public string Status { get; set; }
        public DateTime? CloseDate { get; set; }
    };

    public class LaboratoryExaminationList
    {
        public string Result { get; set; }
        public string DoctorComment { get; set; }
        public DateTime? ExaminationDate { get; set; }
        public string ManagerComment { get; set; }
        public string Status { get; set; }
        public string ExaminationName { get; set; }

    };
    public class PhysicalExaminationList
    {
        public string Result { get; set; }
        public string ExaminationName { get; set; }

    };

    public class ExaminationsDictionaryList
    {
        public int ExaminationDictionaryId { get; set; }
        public string Name { get; set; }
    };

    public struct VisitCancelModel
    {
        public string Reason { get; set; }
    }

    public class AuthenticateModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
    }
}