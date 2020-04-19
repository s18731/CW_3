using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CW_3_v2.Models;

namespace CW_3_v2.Services
{
    public class SqlServerDbService : IStudentsDbService
    {
        private static string connectionString;

        static SqlServerDbService ()
        {
            connectionString = "Data Source=db-mssql;Initial Catalog=s18731;Integrated Security=True";
        }

        public bool isStudentNumberUnique(string studentNumber)
        {
            /* uniqueness of index number */
                using (var client = new SqlConnection(connectionString))
                using (var com = new SqlCommand())
                {
                    com.Connection = client;
                    com.CommandText = "SELECT COUNT(*) as 'isUnique' FROM Student WHERE Student.IndexNumber = @postedIndexNumber";
                    com.Parameters.AddWithValue("postedIndexNumber", studentNumber);

                    client.Open();
                    var dr = com.ExecuteReader();
                    dr.Read();

                    if (Int32.Parse(dr["isUnique"].ToString()) == 1)
                    {
                        client.Close();
                        return false;
                    }
                    else
                    {
                        client.Close();
                        return true;
                    }
                }
                /* uniqueness of index number */
        }

        public bool studiesAvailable (string studiesName)
        {
            /* checking if database contains studies with given name */
                using (var client = new SqlConnection(connectionString))
                using (var com = new SqlCommand())
                {
                    com.Connection = client;
                    com.CommandText = "SELECT COUNT(*) as 'containsStudies' FROM Studies WHERE Studies.Name = @postedStudiesName";
                    com.Parameters.AddWithValue("postedStudiesName", studiesName);

                    client.Open();
                    var dr = com.ExecuteReader();
                    dr.Read();

                    if (Int32.Parse(dr["containsStudies"].ToString()) == 1)
                    {
                        client.Close();
                        return false;
                    }
                    else
                    {
                        client.Close();
                        return true;
                    }
                }
                /* checking if database contains studies with given name */
        }

        public bool CreateEnrollment (Student student)
        {
            using (var client = new SqlConnection(connectionString))
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
                    using (var client2 = new SqlConnection(connectionString))
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
                            return false;
                        }

                        client2.Close();
                    }
                    /* insert operation we perform if table Enrollments does not contain given study */
                }
                else
                {
                    /* inserting student */
                    using (var client2 = new SqlConnection(connectionString))
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
                            return false;
                        }

                        client2.Close();
                    }
                    /* inserting student */
                }

                client.Close();
            }
            return true;
        }

        public Enrollment ReturnedEnrollmentResult(Student student)
        {
            var returnedEnroll = new Enrollment();
            using (var client = new SqlConnection(connectionString))
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

            return returnedEnroll;
        }

        public bool ConatinsEnrollment(EnrollmentPost enrollmentPost)
        {
            using (var client = new SqlConnection(connectionString))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = "SELECT COUNT(*) as 'containsEnroll' FROM Enrollment WHERE Enrollment.IdStudy = (SELECT DISTINCT IdStudy FROM Studies WHERE Studies.Name = @postedStudiesName) AND Enrollment.Semester = @semsterNo";
                com.Parameters.AddWithValue("postedStudiesName", enrollmentPost.Studies);
                com.Parameters.AddWithValue("semesterNo", enrollmentPost.Semester);

                client.Open();
                var dr = com.ExecuteReader();
                dr.Read();

                if (Int32.Parse(dr["containsEnroll"].ToString()) == 1)
                {
                    client.Close();
                    return false;
                }

                client.Close();
                return true;
            }
        }

        public Enrollment PromotedEnrollmentStudent(PromotePost promotePost)
        {
            var returnedEnroll = new Enrollment();
                using (var client2 = new SqlConnection(connectionString))
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
            return returnedEnroll;
        }

        public bool ExecuteDatabaseProcedurePromote(PromotePost promotePost)
        {
            using (var client = new SqlConnection(connectionString))
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
                    return false;
                }

                client.Close();
                return true;
            }
        }
    }
}
