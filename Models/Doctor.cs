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
}