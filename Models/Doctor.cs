using System;
using System.ComponentModel.DataAnnotations;

namespace BackendProject.Models
{
    public class DoctorModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
    };

    public class VisitModel
    {
        public int Id { get; set; }
        public PatientModel Patient { get; set; }
        public DateTime RegisterDate { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0}, Patient: {1}, Reg Date: {2}", Id, Patient, RegisterDate);
        }
    }
}