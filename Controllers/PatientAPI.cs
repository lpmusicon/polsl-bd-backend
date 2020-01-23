using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BackendProject.Models;
using Microsoft.AspNetCore.Authorization;

namespace BackendProject.Controllers
{
    [ApiController]
    [Route("patient")]
    public class PatientController : ControllerBase
    {

        [HttpGet]
        [HttpGet("all")]
        [Authorize(Roles = "RECP, DOCT")]
        public List<Patient> All()
        {
            return new DatabaseContext().Patients.ToList();
        }

        /*
        {
            "Name": "",
            "Lastname": "",
            "PESEL": ""
        }
        */
        [HttpPost("register")]
        [Authorize(Roles = "RECP")]
        public IActionResult Register(PatientRegisterModel input)
        {
            using var db = new DatabaseContext();
            // tutaj sprawdzanie czy login jest unikalny
            if (db.Patients.SingleOrDefault(x => x.PESEL == input.PESEL) != null)
                return BadRequest("Duplicate PESEL");

            if (input.PESEL.Length == 11 && input.PESEL.All(char.IsDigit))
            {
                db.Patients.Add(new Patient(){
                    PESEL = input.PESEL,
                    Name = input.Name,
                    Lastname = input.Lastname
                });
                db.SaveChanges();
                return NoContent();
            }
            return BadRequest();
        }

        [HttpGet("{patientId}/visit")]
        [HttpGet("{patientId}/visit/all")]
        [Authorize(Roles = "RECP, DOCT")]
        public List<PatientVisitsModel> Visits(int patientId)
        {
            using var db = new DatabaseContext();
            var result = (
                from p in db.Patients
                join pv in db.PatientVisits on p.PatientId equals pv.PatientId
                join d in db.Doctors on pv.DoctorId equals d.DoctorId
                where pv.PatientId == patientId
                select new PatientVisitsModel
                {
                    Patient = new PatientModel() {
                        PatientId = p.PatientId,
                        Name = p.Name,
                        Lastname = p.Lastname
                    },
                    RegisterDate = pv.RegisterDate,
                    PatientVisitId = pv.PatientVisitId,
                    CloseDate = pv.CloseDate,
                    Description = pv.Description,
                    Diagnosis = pv.Diagnosis,
                    Doctor = new DoctorModel() {
                        DoctorId = d.DoctorId,
                        Name = d.Name,
                        Lastname = d.Lastname
                    },
                    Status = pv.Status
                }).ToList();

            return result;
        }

        [HttpGet("{patientId}/physical_examinations/all")]
        [Authorize(Roles = "DOCT")]
        public List<PatientPhysicalExaminationsModel> PatientPhysicalExaminations(int patientId)
        {
            using var db = new DatabaseContext();
            var result = (
                from p in db.Patients
                join pv in db.PatientVisits on p.PatientId equals pv.PatientId
                join pe in db.PhysicalExaminations on pv.PatientVisitId equals pe.PatientVisitId
                join ed in db.ExaminationsDictionary on pe.ExaminationDictionaryId equals ed.ExaminationDictionaryId
                join d in db.Doctors on pv.DoctorId equals d.DoctorId
                where pv.PatientId == patientId
                select new PatientPhysicalExaminationsModel
                {
                    ExaminationName = ed.Name,
                    Result = pe.Result,
                    DoctorName = d.Name,
                    DoctorLastName = d.Lastname,
                    ExaminationDate = pv.CloseDate
                }).ToList();

            return result;
        }

        [HttpGet("{patientId}/laboratory_examinations/all")]
        [Authorize(Roles = "DOCT")]
        public List<PatientLaboratoryExaminationsModel> PatientLaboratoryExaminations(int patientId)
        {
            using var db = new DatabaseContext();
            var result = (
                from p in db.Patients
                join pv in db.PatientVisits on p.PatientId equals pv.PatientId
                join le in db.LaboratoryExaminations on pv.PatientVisitId equals le.PatientVisitId
                join ed in db.ExaminationsDictionary on le.ExaminationDictionaryId equals ed.ExaminationDictionaryId
                join d in db.Doctors on pv.DoctorId equals d.DoctorId
                where pv.PatientId == patientId
                select new PatientLaboratoryExaminationsModel
                {
                    ExaminationName = ed.Name,
                    Result = le.Result,
                    DoctorName = d.Name,
                    DoctorLastName = d.Lastname,
                    OrderExaminationDate = le.OrderDate,
                    ExecuteExaminationDate = le.ExaminationDate,
                    Status = le.Status
                }).ToList();

            return result;
        }
    }
}