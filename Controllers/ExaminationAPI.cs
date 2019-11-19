using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using BackendProject.Models;

namespace BackendProject.Controllers
{
    [ApiController]
    [Route("examination/dictionary")]
    public class ExaminationsDictionaryController : ControllerBase
    {
        [HttpGet("{type}")]
        public string Get(char type)
        { // jesli chcesz fizykalne daj doctor/get_examination_dictionary/F jak laboratoryjne doctor/get_examination_dictionary/L
            using var db = new DatabaseContext();
            switch (type)
            {
                case 'F':
                    var resultf = (from x in db.ExaminationsDictionary
                                   where x.Type == 'F'
                                   select new ExaminationsDictionaryList { ExaminationDictionaryId = x.ExaminationDictionaryId, Name = x.Name }).ToList();
                    return JsonSerializer.Serialize<List<ExaminationsDictionaryList>>(resultf);

                case 'L':
                    var resultl = (from x in db.ExaminationsDictionary
                                   where x.Type == 'L'
                                   select new ExaminationsDictionaryList { ExaminationDictionaryId = x.ExaminationDictionaryId, Name = x.Name }).ToList();
                    return JsonSerializer.Serialize<List<ExaminationsDictionaryList>>(resultl);

                default:
                    return "Bad argument";
            }
        }
    }
}