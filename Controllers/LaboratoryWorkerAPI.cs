using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using BackendProject.Models;

namespace BackendProject.Controllers
{
    [ApiController]
    [Route("laboratory_worker/examination_execute")]
    /*
    {
    "LaboratoryExaminationId": ,
    "Result": "",
    "LaboratoryWorkerId": 
    }
    */
    public class ExaminationExecuteController : ControllerBase
    {
        public IActionResult Post(ExaminationExecute input)
        {
            using var db = new DatabaseContext();
            var ex = db.LaboratoryExaminations.SingleOrDefault(x => x.LaboratoryExaminationId == input.LaboratoryExaminationId);
            if (ex != null && input.Result != null)
            {
                ex.Status = "Executed";
                ex.Result = input.Result;
                ex.LaboratoryWorkerId = input.LaboratoryWorkerId;
                ex.ExaminationDate = DateTime.Now;
                db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }
    }

    [ApiController]
    [Route("laboratory_worker/examination_cancel")]
    /*
    {
        "LaboratoryExaminationsId": ,
        "LaboratoryWorkerId": 
    }
    */
    public class ExaminationCancelController : ControllerBase
    {
        public IActionResult Post(ExaminationCancel input)
        {
            using var db = new DatabaseContext();
            var ex = db.LaboratoryExaminations.SingleOrDefault(x => x.LaboratoryExaminationId == input.LaboratoryExaminationsId);
            if (ex != null)
            {
                ex.Status = "Canceled";
                ex.ExaminationDate = DateTime.Now;
                ex.LaboratoryWorkerId = input.LaboratoryWorkerId;
                db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }
    }

    [ApiController]
    [Route("laboratory_worker/get_ordered_examinations_list/")]
    [Route("laboratory_manager/get_ordered_examinations_list/")]
    public class GetOrderedExaminationsListController : ControllerBase
    {
        public string Get()
        {
            using var db = new DatabaseContext();
            var result = (from le in db.LaboratoryExaminations
                          join ed in db.ExaminationsDictionary on le.ExaminationDictionaryId equals ed.ExaminationDictionaryId
                          where le.Status == "Ordered"
                          select new OrderedExaminationList
                          {
                              LaboratoryExaminatonName = ed.Name,
                              DoctorComment = le.DoctorComment,
                              OrderDate = le.OrderDate,
                          }).ToList();
            return JsonSerializer.Serialize<List<OrderedExaminationList>>(result);
        }
    }
}