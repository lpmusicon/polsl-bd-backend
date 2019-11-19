using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using BackendProject.Models;

namespace BackendProject.Controllers
{
    [ApiController]
    [Route("doctor")]
    [Route("doctor/all")]
    public class DoctorController : ControllerBase
    {
        public string Get()
        {
            using var db = new DatabaseContext();
            var result = (from x in db.Doctors select new DoctorsList { 
                DoctorId = x.DoctorId, 
                Name = x.Name, 
                Lastname = x.Lastname }).ToList();

            return JsonSerializer.Serialize<List<DoctorsList>>(result);
        }
    }


    /* 
    {
    "ExaminationDictionaryId": , 
    "PatientVisitId": ,
    "Result": ""  
    }
    */
    [ApiController]
    [Route("doctor/physical_examination")]
    public class PsychicalExaminationController : ControllerBase
    {
        public IActionResult Post(PhysicalExamination input)
        {

            using var db = new DatabaseContext();
            var pvcheck = db.PatientVisits.SingleOrDefault(x => x.PatientVisitId == input.PatientVisitId);
            var pecheck = db.ExaminationsDictionary.SingleOrDefault(x => x.ExaminationDictionaryId == input.ExaminationDictionaryId);
            bool isExaminationValid = input.PhysicalExaminationId == 0 && pvcheck != null && pvcheck.Status == "Registered" && pecheck.Type == 'F';
            if (isExaminationValid)
            {
                db.Add(input);
                db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }
    }

    /* 
    {
    "DoctorComment": "", 
    "PatientVisitId": , 
    "ExaminationDictionaryId":  
    }
    */
    [ApiController]
    [Route("doctor/order_laboratory_examination")]
    public class OrderLaboratoryExaminationController : ControllerBase
    {
        public IActionResult Post(LaboratoryExamination input)
        {

            using var db = new DatabaseContext();
            var pecheck = db.ExaminationsDictionary.SingleOrDefault(x => x.ExaminationDictionaryId == input.ExaminationDictionaryId);
            if (input.LaboratoryExaminationId == 0 && db.PatientVisits.SingleOrDefault(x => x.PatientVisitId == input.PatientVisitId) != null && pecheck.Type == 'L' &&
            input.ManagerComment == null && input.ApprovalRejectionDate == null && input.Result == null)
            {
                input.OrderDate = DateTime.Now;
                input.Status = "Ordered";
                db.Add(input);
                db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }
    }

    [ApiController]
    [Route("doctor/get_doctors_patient_visits")]
    public class GetDoctorsPatientVisitsController : ControllerBase
    {
        [HttpGet("{doctorId}")]
        public string Get(int doctorId)
        {
            using var db = new DatabaseContext();
            var result = (from p in db.Patients
                          join pv in db.PatientVisits on p.PatientId equals pv.PatientId
                          where pv.DoctorId == doctorId && pv.Status == "Registered"
                          select new PatientsVisitsList
                          {
                              PatientName = p.Name,
                              PatientLastname = p.Lastname,
                              RegisterDate = pv.RegisterDate,
                              PatientId = pv.PatientId
                          }).ToList();

            return JsonSerializer.Serialize<List<PatientsVisitsList>>(result);
        }
        public string Get()
        {
            using var db = new DatabaseContext();
            var result = (from p in db.Patients
                          join pv in db.PatientVisits on p.PatientId equals pv.PatientId
                          join d in db.Doctors on pv.DoctorId equals d.DoctorId
                          where pv.Status == "Registered"
                          select new AllPatientsVisitsList
                          {
                              DoctorName = d.Name,
                              DoctorLastname = d.Lastname,
                              PatientName = p.Name,
                              PatientLastname = p.Lastname,
                              RegisterDate = pv.RegisterDate,
                              PatientId = pv.PatientId
                          }).ToList();

            return JsonSerializer.Serialize<List<AllPatientsVisitsList>>(result);
        }
    }

    [ApiController]
    [Route("doctor/get_patient_laboratory_examinations_list")]
    public class GetPatientLaboratoryExaminationsListController : ControllerBase
    {
        [HttpGet("{PatientVisitId}")]
        public string Get(int PatientVisitId)
        {
            using var db = new DatabaseContext();
            var result = (from le in db.LaboratoryExaminations
                          join ed in db.ExaminationsDictionary on le.ExaminationDictionaryId equals ed.ExaminationDictionaryId
                          where le.PatientVisitId == PatientVisitId
                          select new LaboratoryExaminationList
                          {
                              Result = le.Result,
                              DoctorComment = le.DoctorComment,
                              ExaminationDate = le.ExaminationDate,
                              Status = le.Status,
                              ManagerComment = le.ManagerComment,
                              ExaminationName = ed.Name
                          }).ToList();

            return JsonSerializer.Serialize<List<LaboratoryExaminationList>>(result);
        }
    }

    [ApiController]
    [Route("doctor/get_patient_physical_examinations_list")]
    public class GetPatientPhysicalExaminationsListController : ControllerBase
    {
        [HttpGet("{PatientVisitId}")]
        public string Get(int PatientVisitId)
        {
            using var db = new DatabaseContext();
            var result = (from pe in db.PhysicalExaminations
                          join ed in db.ExaminationsDictionary on pe.ExaminationDictionaryId equals ed.ExaminationDictionaryId
                          where pe.PatientVisitId == PatientVisitId
                          select new PhysicalExaminationList { ExaminationName = ed.Name, Result = pe.Result }).ToList();

            return JsonSerializer.Serialize<List<PhysicalExaminationList>>(result);
        }
    }
}
