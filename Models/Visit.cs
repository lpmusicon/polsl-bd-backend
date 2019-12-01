using System;
using System.ComponentModel.DataAnnotations;

    public class RegisterVisitModel{
        [Required]
        public int PatientId { get; set; }
        [Required]
        public int DoctorId { get; set; }
        public DateTime RegisterDate { get; set; }
    }

    public class PatientVisitCloseModel
    {
        public string Description { get; set; }
        public string Diagnosis { get; set; }
    };