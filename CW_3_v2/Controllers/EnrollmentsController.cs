using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CW_3_v2.Models;
using CW_3_v2.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CW_3_v2.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly Services.IStudentsDbService _dbService;
        
        public EnrollmentsController(IStudentsDbService studentsDbService)
        {
            _dbService = studentsDbService;
        }

        [HttpPost]
        public IActionResult InsertStudent(EnrollmentPost student)
        {
             /* uniqueness of index number */
            if (student.IndexNumber == null || !_dbService.IsStudentNumberUnique(student.IndexNumber))
                return BadRequest("Student with given index number already exists in the database.");
            
            /* checking if database contains studies with given name */
            if (student.Studies == null || !_dbService.StudiesAvailable(student.Studies))               
                return BadRequest("Database does not contain studies with given name.");

            /* enrollments */
            if (!_dbService.CreateEnrollment(student))
                return BadRequest("Failed to create enrollment.");

            /* returned result */
            return Created("Student created", _dbService.ReturnedEnrollmentResult(student));
        }

        [HttpPost("{promotions}")]
        public IActionResult Promote(PromotePost studies)
        {
            /* checking if database conatins enrollment with given parameters */
            if (!_dbService.ConatinsEnrollment(studies)) 
                return BadRequest("Database does not contain enroll with given parameters.");

            /* executing procedure */
            if(!_dbService.ExecuteDatabaseProcedurePromote(studies))
                return BadRequest("Failed to promote students. Restoring changes.");
            /* returned result */
            return Created("Enroll for updated students", _dbService.ConatinsEnrollment(studies));
        }
       
    }
}
