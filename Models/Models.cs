using System;
using System.ComponentModel.DataAnnotations;

namespace BackendProject.Models
{
    public struct ReasonModel
    {
        public string Reason { get; set; }
    }

    public class VisitModel
    {
        public int PatientVisitId { get; set; }
        public PatientModel Patient { get; set; }
        public DateTime RegisterDate { get; set; }
    };

    public class GenericVisitModel : VisitModel
    {
        public string Diagnosis { get; set; }
        public string Description { get; set; }
        public DateTime? CloseDate { get; set; }
        public string Status { get; set; }
        public DoctorModel Doctor { get; set; }
    }

    public class AllPatientsVisitsModel : VisitModel
    {
        public DoctorModel Doctor { get; set; }
    };

    public class PatientVisitsModel : AllPatientsVisitsModel
    {
        public string Description { get; set; }
        public string Diagnosis { get; set; }
        public string Status { get; set; }
        public DateTime? CloseDate { get; set; }
    };
}