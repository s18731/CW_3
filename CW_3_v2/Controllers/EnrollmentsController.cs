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
        public IActionResult InsertStudent(EnrollmentPost student)
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

            /* enrollments */
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18731;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = "SELECT TOP 1 IdStudy FROM Enrollment WHERE Enrollment.Semester = 1 ORDER BY Enrollment.StartDate;";
                com.Parameters.AddWithValue("postedStudiesName", student.Studies);

                client.Open();
                var dr = com.ExecuteReader();
                if (!dr.HasRows)
                {
                    /* insert operation we perform if table Enrollments does not contain given study */
                    DateTime currentDateTime = DateTime.Now;
                    string formattedDate = currentDateTime.ToString("d");
                    using (var client2 = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18731;Integrated Security=True"))
                    using (var com2 = new SqlCommand())
                    {
                        SqlTransaction transaction = client2.BeginTransaction("Transaction");
                        com2.Connection = client;
                        com2.CommandText = "INSERT INTO Enrollment(IdEnrollment, Semester, IdStudy, StartDate) VALUES (SELECT MAX(Enrollment.IdEnrollment) FROM Enrollment + 1, 1, SELECT DISTINCT IdStudy FROM Studies WHERE Studies.Name = @courseName, @currDate)";
                        com2.Parameters.AddWithValue("courseName", student.Studies);
                        com2.Parameters.AddWithValue("currDate", formattedDate);

                        client2.Open();
                        try
                        {
                            var nonq = com2.ExecuteNonQuery();
                        }
                        catch (SqlException e)
                        {
                            transaction.Rollback();
                            return BadRequest();
                        }

                        client2.Close();
                    }
                    /* insert operation we perform if table Enrollments does not contain given study */
                }
                else
                {
                    /* inserting student */
                    using (var client2 = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18731;Integrated Security=True"))
                    using (var com2 = new SqlCommand())
                    {
                        SqlTransaction transaction = client2.BeginTransaction("Transaction");
                        com2.Connection = client;
                        com2.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate) VALUES (@IndexNumber, @FirstName, @LastName, @BirthDate, SELECT DISTINCT IdStudy FROM Studies WHERE Studies.Name = @courseName)";

                        com2.Parameters.AddWithValue("IndexNumber", student.IndexNumber);
                        com2.Parameters.AddWithValue("FirstName", student.FirstName);
                        com2.Parameters.AddWithValue("LastName", student.LastName);
                        com2.Parameters.AddWithValue("BirthDate", student.BirthDate);
                        com2.Parameters.AddWithValue("courseName", student.Studies);

                        client2.Open();

                        try
                        {
                            var nonq = com2.ExecuteNonQuery();
                        }
                        catch (SqlException e)
                        {
                            transaction.Rollback();
                            return BadRequest();
                        }

                        client2.Close();
                    }
                    /* inserting student */
                }

                client.Close();
            }
            /* enrollments */

            /* returned result */

            var returnedEnroll = new Enrollment();
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18731;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = "SELECT DISTINCT * FROM Enrollment WHERE Enrollment.IdEnrollment = (SELECT DISTINCT IdStudy FROM Studies WHERE Studies.Name = @courseName)";
                com.Parameters.AddWithValue("courseName", student.Studies);

                client.Open();
                var dr = com.ExecuteReader();
                dr.Read();

                returnedEnroll.IdEnrollment = Int32.Parse(dr["IdEnrollment"].ToString());
                returnedEnroll.IdStudy = Int32.Parse(dr["IdStudy"].ToString());
                returnedEnroll.Semester = Int32.Parse(dr["Semester"].ToString());
                returnedEnroll.StartDate = dr["StartDate"].ToString();

                client.Close();
            }

            /* returned result */
            return Created("Student created", returnedEnroll);
        }

        [HttpPost("{promotions}")]
        public IActionResult Promote(PromotePost studies)
        {
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18731;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = "SELECT COUNT(*) as 'containsEnroll' FROM Enrollment WHERE Enrollment.IdStudy = (SELECT DISTINCT IdStudy FROM Studies WHERE Studies.Name = @postedStudiesName) AND Enrollment.Semester = @semsterNo";
                com.Parameters.AddWithValue("postedStudiesName", studies.Studies);
                com.Parameters.AddWithValue("semesterNo", studies.Semester);

                client.Open();
                var dr = com.ExecuteReader();
                dr.Read();

                if (Int32.Parse(dr["containsEnroll"].ToString()) == 1)
                    return BadRequest("Database does not contain enroll with given parameters.");

                client.Close();
            }

            /* executing procedure */
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18731;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                SqlTransaction transaction = client.BeginTransaction("Transaction");
                com.CommandText = "EXECUTE promoteForNextSemester @studies , @semester";
                com.Parameters.AddWithValue("studies", studies.Studies);
                com.Parameters.AddWithValue("semester", studies.Semester);
                try
                {
                    var nonq = com.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    transaction.Rollback();
                    return BadRequest();
                }

                client.Close();
                /* executing procedure */

                /* returned result */

                var returnedEnroll = new Enrollment();
                using (var client2 = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18731;Integrated Security=True"))
                using (var com2 = new SqlCommand())
                {
                    com2.Connection = client2;
                    com2.CommandText = "SELECT DISTINCT * FROM Enrollment WHERE Enrollment.IdStudy = (SELECT IdStudy FROM Study WHERE Study.Name = @Studies)";
                    com2.Parameters.AddWithValue("courseName", studies.Studies);

                    client2.Open();
                    var dr2 = com2.ExecuteReader();
                    dr2.Read();

                    returnedEnroll.IdEnrollment = Int32.Parse(dr2["IdEnrollment"].ToString());
                    returnedEnroll.IdStudy = Int32.Parse(dr2["IdStudy"].ToString());
                    returnedEnroll.Semester = Int32.Parse(dr2["Semester"].ToString());
                    returnedEnroll.StartDate = dr2["StartDate"].ToString();

                    client2.Close();
                }

                /* returned result */

                return Created("Enroll for updated students", returnedEnroll);
            }
        }
       
    }
}
