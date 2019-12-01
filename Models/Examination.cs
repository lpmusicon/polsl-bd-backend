using System;
using System.ComponentModel.DataAnnotations;

namespace BackendProject.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
    }
    public class DictionaryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}

namespace BackendProject.Models.Physical
{
    public class PerformExaminationModel
    {
        public string Result { get; set; }
        public int VisitId { get; set; }
        public int ExaminationId { get; set; }
    }

    public class PhysicalExaminationModel
    {
        public string Name { get; set; }
        public string Result { get; set; }
    }
}

namespace BackendProject.Models.Laboratory
{
    public class Executed
    {
        public string Result { get; set; }
        public string DoctorComment { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ExaminationDate { get; set; }
        public string Status { get; set; }
        public Person Worker { get; set; }
        public string ExaminationName { get; set; }
    };

    public class Resolved : Executed
    {
        public Person Manager { get; set; }
        public DateTime? ApprovalRejectionDate { get; set; }
    };

    public class Ordered
    {
        public int Id { get; set; }
        public string DoctorComment { get; set; }
        public string LaboratoryExaminationName { get; set; }
        public DateTime OrderDate { get; set; }
    };

    public class OrderedExamination
    {
        public int Id { get; set; }
        public string Result { get; set; }
        public string DoctorComment { get; set; }
        public DateTime? ExaminationDate { get; set; }
        public string ManagerComment { get; set; }
        public string Status { get; set; }
        public string ExaminationName { get; set; }
    };

    public class OrderExaminationModel
    {
        public int VisitId { get; set; }
        public int ExaminationTypeId { get; set; }
        public string DoctorComment { get; set; }
    }

    public class ResultModel
    {
        public string Result { get; set; }
    };
}