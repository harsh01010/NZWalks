using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NZWalks.API.Controllers
{
    //https://localhost:portnumber/api/students
    [Route("api/[controller]")]
    [ApiController]

    public class StudentsController : ControllerBase
    {
        //get:https://localhost:portnumber/api/students
        [HttpGet]
        public IActionResult GetAllStudents()
        {
            var students = new List<string>() { "student 1", "student 2", "student 3", "student 4" };
            return Ok(students);
        }
    }
}
