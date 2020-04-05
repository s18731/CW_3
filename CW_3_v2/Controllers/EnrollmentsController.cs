using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CW_3_v2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CW_3_v2.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        [HttpPost]
        public IActionResult InsertStudent(EnrolmentPost student)
        {
            if (student.IndexNumber != null)
            {
                /* uniqueness of index number */
                    using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18731;Integrated Security=True"))
                    using (var com = new SqlCommand())
                    {
                        com.Connection = client;
                        com.CommandText = "SELECT COUNT(*) as 'isUnique' FROM Student WHERE Student.IndexNumber = @postedIndexNumber";
                        com.Parameters.AddWithValue("postedIndexNumber", student.IndexNumber);

                        client.Open();
                        var dr = com.ExecuteReader();
                        dr.Read();

                    if (Int32.Parse(dr["isUnique"].ToString()) == 1)
                        return BadRequest("Student with given index number already exists in the database.");

                        client.Close();
                    }
                /* uniqueness of index number */
            }

            if (student.Studies != null)
            {
                /* checking if database contains studies with given name */
                using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18731;Integrated Security=True"))
                using (var com = new SqlCommand())
                {
                    com.Connection = client;
                    com.CommandText = "SELECT COUNT(*) as 'containsStudies' FROM Studies WHERE Studies.Name = @postedStudiesName";
                    com.Parameters.AddWithValue("postedStudiesName", student.Studies);

                    client.Open();
                    var dr = com.ExecuteReader();
                    dr.Read();

                    if (Int32.Parse(dr["containsStudies"].ToString()) == 1)
                        return BadRequest("Database does not contain studies with given name.");

                    client.Close();
                }
                /* checking if database contains studies with given name */
            }


            var enrollment = new Enrollment();
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18731;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = "SELECT * FROM Student";

                client.Open();
                var dr = com.ExecuteReader();
                dr.Read();

                client.Close();
            }
            return Ok(enrollment);
        }
    }
}
