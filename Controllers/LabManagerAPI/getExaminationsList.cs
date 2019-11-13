using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization; 

namespace bd_backend.Controllers{   
    public class LEL{
        public string result { get; set; }
        public string doctorComment { get; set; }
        public DateTime orderDate { get; set; }
        public DateTime? examinationDate { get; set; }
        public string status { get; set; }
        public string labWorkerName { get; set; }
        public string labWorkerLastname { get; set; }

    };
    [ApiController]
    [Route("api/labmanager/getexaminationslist/")]
    public class getExaminationsListController : ControllerBase{
    public string Get(){
            using (var db = new DatabaseContext()){
                var result = (from le in db.LaboratoryExaminations join lw in db.LaboratoryWorkers on le.LaboratoryWorkerId equals lw.LaboratoryWorkerId 
                where le.Status == "Executed" select  new LEL { result = le.Result, doctorComment = le.DoctorComment, orderDate = le.OrderDate, 
                examinationDate = le.ExaminationDate, status = le.Status, labWorkerName = lw.Name, labWorkerLastname = lw.Lastname}).ToList();  
            return JsonSerializer.Serialize<System.Collections.Generic.List<bd_backend.Controllers.LEL>>(result);
            }
        }
    }
}