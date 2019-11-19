using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using BackendProject.Models;

namespace BackendProject.Controllers
{
    [ApiController]
    [Route("examination")]
    public class ExaminationController : ControllerBase
    {
        [HttpGet("{type}")]
        public List<ExaminationsDictionaryList> Get(char type)
        { // jesli chcesz fizykalne daj doctor/get_examination_dictionary/F jak laboratoryjne doctor/get_examination_dictionary/L
            using var db = new DatabaseContext();
            switch (type)
            {
                case 'F':
                    var result = (from x in db.ExaminationsDictionary
                                   where x.Type == 'F'
                                   select new ExaminationsDictionaryList { 
                                       ExaminationDictionaryId = x.ExaminationDictionaryId, 
                                       Name = x.Name }).ToList();
                    return result;

                case 'L':
                    var resultL = (from x in db.ExaminationsDictionary
                                   where x.Type == 'L'
                                   select new ExaminationsDictionaryList { 
                                       ExaminationDictionaryId = x.ExaminationDictionaryId, 
                                       Name = x.Name }).ToList();
                    return resultL;

                default:
                    return new List<ExaminationsDictionaryList>();
            }
        }

        
    }
}