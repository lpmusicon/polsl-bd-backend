using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using bd_backend.Interface;

namespace bd_backend.Controllers{

    [ApiController]
    [Route("doctor/do_patient_visit")]
    /* 
    {
    "PatientVisitId": , 
    "DoctorID": , 
    "Description": "",
    "Diagnosis": ""  
    }
    */
    public class DoPatientVisitController : ControllerBase{
        public IActionResult Post(PatientVisitForm input){
            
            if(input.PatientVisitId != 0 && input.DoctorId != 0 && input.Description != null && input.Diagnosis != null){ // chyba musial wpisac opis, dowiemy sie
                // Biere login
                using (var db = new DatabaseContext()){
                    var pv = db.PatientVisits.SingleOrDefault(x => x.PatientVisitId == input.PatientVisitId); 
                    if(pv != null && pv.Status == "Registered" && db.Doctors.SingleOrDefault(x => x.DoctorId == input.DoctorId) != null ){
                        pv.Diagnosis = input.Diagnosis;
                        pv.Description = input.Description;
                        pv.DoctorId = input.DoctorId; 
                        pv.CloseDate = DateTime.Now;
                        pv.Status = "Closed";
                        db.SaveChanges();
                        return Ok();
                    }else return NotFound();
                } 
            }else return BadRequest();
        }
    }

    [ApiController]
    [Route("doctor/physical_examination")]
    /* 
    {
    "ExaminationDictionaryId": , 
    "PatientVisitId": ,
    "Result": ""  
    }
    */
    public class PsychicalExaminationController : ControllerBase{
        public IActionResult Post(PhysicalExamination input){
            
            using (var db = new DatabaseContext()){
                var pvcheck = db.PatientVisits.SingleOrDefault(x => x.PatientVisitId == input.PatientVisitId);
                var pecheck = db.ExaminationsDictionary.SingleOrDefault(x => x.ExaminationDictionaryId == input.ExaminationDictionaryId);
                if(input.PhysicalExaminationId == 0 && pvcheck != null && pvcheck.Status == "Registered" && pecheck.Type == 'F'){
                    db.Add(input);
                    db.SaveChanges();
                    return Ok();
                }else return BadRequest();
            } 
        }
    }

    [ApiController]
    [Route("doctor/order_laboratory_examination")]
    /* 
    {
    "DoctorComment": "", 
    "PatientVisitId": , 
    "ExaminationDictionaryId":  
    }
    */
    public class OrderLaboratoryExaminationController : ControllerBase{
        public IActionResult Post(LaboratoryExamination input){
            
            using (var db = new DatabaseContext()){
                var pecheck = db.ExaminationsDictionary.SingleOrDefault(x => x.ExaminationDictionaryId == input.ExaminationDictionaryId);
                if(input.LaboratoryExaminationId == 0 && db.PatientVisits.SingleOrDefault(x => x.PatientVisitId == input.PatientVisitId) != null && pecheck.Type == 'L' && 
                input.ManagerComment == null && input.ApprovalRejectionDate == null && input.Result == null){
                    input.OrderDate = DateTime.Now;
                    input.Status = "Ordered";
                    db.Add(input);
                    db.SaveChanges();
                    return Ok();
                }else return BadRequest();
            } 
        }
    }

    [ApiController]
    [Route("doctor/patient_visit_cancel")]
    /* 
    {
    "PatientVisitId": , 
    "Description": ""
    }
    */
    public class PatientVisitDoctorCancelController : ControllerBase{
        public IActionResult Post(PatientVisitCancel input){
            using (var db = new DatabaseContext()){
                // anuluje tylko swoje wizyty? jesli tak, dopisz cos, teraz moze anulowac wszystko (Kononowicz mode)
                var pv = db.PatientVisits.SingleOrDefault(x => x.PatientVisitId == input.PatientVisitId);
                
                if(pv != null && input.Description != null){
                    pv.Status = "Canceled";
                    pv.CloseDate = DateTime.Now;
                    pv.Description = input.Description;
                    db.SaveChanges();
                    return Ok();
                }return BadRequest();
            }
        }
    }

    [ApiController]
    [Route("doctor/get_doctors_patient_visits")]
    public class GetDoctorsPatientVisitsController : ControllerBase{
        [HttpGet("{doctorId}")]
        public string Get(int doctorId){
            using (var db = new DatabaseContext()){
            var result = (from p in db.Patients join pv in db.PatientVisits on p.PatientId equals pv.PatientId
            where pv.DoctorId == doctorId && pv.Status == "Registered" select new PatientsVisitsList { PatientName = p.Name, PatientLastname = p.Lastname, 
            RegisterDate = pv.RegisterDate, PatientId = pv.PatientId }).ToList();  
            
            return JsonSerializer.Serialize<List<PatientsVisitsList>>(result);
            }
        }
    public string Get(){
            using (var db = new DatabaseContext()){
            var result = (from p in db.Patients join pv in db.PatientVisits on p.PatientId equals pv.PatientId
            join d in db.Doctors on pv.DoctorId equals d.DoctorId
            where pv.Status == "Registered" select  new AllPatientsVisitsList { DoctorName = d.Name, DoctorLastname = d.Lastname, PatientName = p.Name, 
            PatientLastname = p.Lastname, RegisterDate = pv.RegisterDate, PatientId = pv.PatientId }).ToList();  
            
            return JsonSerializer.Serialize<List<AllPatientsVisitsList>>(result);
            }
        }
    }

    [ApiController]
    [Route("doctor/get_patient_visits")]
    [Route("recepcion/get_patient_visits")]
    public class GetPatientVisitsController : ControllerBase{
        [HttpGet("{patientId}")]
        public string Get(int patientId){
            using (var db = new DatabaseContext()){
            var result = (from p in db.Patients join pv in db.PatientVisits on p.PatientId equals pv.PatientId
            join d in db.Doctors on pv.DoctorId equals d.DoctorId
            where pv.PatientId == patientId select new PatientVisitsList { PatientName = p.Name, PatientLastname = p.Lastname, RegisterDate = pv.RegisterDate, PatientVisitId = pv.PatientVisitId, 
            CloseDate = pv.CloseDate, Description = pv.Description, Diagnosis = pv.Diagnosis, DoctorName = d.Name, DoctorLastname = d.Lastname, Status = pv.Status, PatientId = patientId }).ToList();  
            
            return JsonSerializer.Serialize<List<PatientVisitsList>>(result);
            }
        }
        public string Get(){
            using (var db = new DatabaseContext()){
            var result = (from p in db.Patients join pv in db.PatientVisits on p.PatientId equals pv.PatientId
            join d in db.Doctors on pv.DoctorId equals d.DoctorId
            select new PatientVisitsList { PatientName = p.Name, PatientLastname = p.Lastname, RegisterDate = pv.RegisterDate, PatientVisitId = pv.PatientVisitId, 
            CloseDate = pv.CloseDate, Description = pv.Description, Diagnosis = pv.Diagnosis, DoctorName = d.Name, DoctorLastname = d.Lastname, Status = pv.Status, PatientId = pv.PatientId }).ToList();  
            
            return JsonSerializer.Serialize<List<PatientVisitsList>>(result);
            }
        }
    }

    [ApiController]
    [Route("doctor/get_patient_laboratory_examinations_list")]
    public class GetPatientLaboratoryExaminationsListController : ControllerBase{
    [HttpGet("{PatientVisitId}")]
    public string Get(int PatientVisitId){
            using (var db = new DatabaseContext()){
                var result = (from le in db.LaboratoryExaminations join ed in db.ExaminationsDictionary on le.ExaminationDictionaryId equals ed.ExaminationDictionaryId
                where le.PatientVisitId == PatientVisitId select new LaboratoryExaminationList { Result = le.Result, DoctorComment = le.DoctorComment, 
                ExaminationDate = le.ExaminationDate, Status = le.Status, ManagerComment = le.ManagerComment, ExaminationName = ed.Name}).ToList();  
                
                return JsonSerializer.Serialize<List<LaboratoryExaminationList>>(result);
            }
        }
    }

    [ApiController]
    [Route("doctor/get_patient_physical_examinations_list")]
    public class GetPatientPhysicalExaminationsListController : ControllerBase{
    [HttpGet("{PatientVisitId}")]
    public string Get(int PatientVisitId){
            using (var db = new DatabaseContext()){
                var result = (from pe in db.PhysicalExaminations join ed in db.ExaminationsDictionary on pe.ExaminationDictionaryId equals ed.ExaminationDictionaryId
                where pe.PatientVisitId == PatientVisitId select new PhysicalExaminationList { ExaminationName = ed.Name, Result = pe.Result }).ToList();  
                
                return JsonSerializer.Serialize<List<PhysicalExaminationList>>(result);
            }
        }
    }

    [ApiController]
    [Route("doctor/get_examination_dictionary")]
    public class GetPhysicalExaminationsController : ControllerBase{
    [HttpGet("{type}")]
    public string Get(char type){ // jesli chcesz fizykalne daj doctor/get_examination_dictionary/F jak laboratoryjne doctor/get_examination_dictionary/L
            using (var db = new DatabaseContext()){
                switch(type){
                    case 'F':
                        var resultf = (from x in db.ExaminationsDictionary where x.Type == 'F' 
                        select new ExaminationsDictionaryList { ExaminationDictionaryId = x.ExaminationDictionaryId, Name = x.Name }).ToList();
                    return JsonSerializer.Serialize<List<ExaminationsDictionaryList>>(resultf);

                    case 'L':
                        var resultl = (from x in db.ExaminationsDictionary where x.Type == 'L' 
                        select new ExaminationsDictionaryList { ExaminationDictionaryId = x.ExaminationDictionaryId, Name = x.Name }).ToList();
                    return JsonSerializer.Serialize<List<ExaminationsDictionaryList>>(resultl);

                    default: return "Bad argument";
                }
            }
        }
    }
}
