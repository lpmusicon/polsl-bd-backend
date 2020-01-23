using System;
using System.ComponentModel.DataAnnotations;

namespace BackendProject.Models 
{
    public class PatientRegisterModel {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Lastname { get; set; }
        [Required]
        [StringLength(11)]
        public string PESEL { get; set; }
    }

    public class PatientModel {
        public int PatientId { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string PESEL { get; set; }
    }

    public class PatientPhysicalExaminationsModel{
        public string ExaminationName { get; set; }
        public string Result { get; set; }
        public string DoctorName { get; set; }
        public string DoctorLastName { get; set; }
        public DateTime? ExaminationDate { get; set; }
    }

    public class PatientLaboratoryExaminationsModel{
        public string ExaminationName { get; set; }
        public string Result { get; set; }
        public string DoctorName { get; set; }
        public string DoctorLastName { get; set; }
        public DateTime OrderExaminationDate { get; set; }
        public string Status { get; set; }
        public DateTime? ExecuteExaminationDate { get; set; }
    }
}