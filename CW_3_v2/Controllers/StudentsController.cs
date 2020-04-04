using System;
using System.Collections.Generic;
using System.Linq;
using CW_3_v2.DAL;
using CW_3_v2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace CW_3_v2.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;
        private readonly string NoIDFoundResponse = "Student with given ID does not exist";

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetStudents(string orderBy)
        {
            List <Student> orderedList = _dbService.GetStudents().ToList();
            switch (orderBy)
            {
                case "FirstName":
                    return Ok(_dbService.GetStudents().ToList().OrderBy(x => x.FirstName));
                case "LastName":
                    return Ok(_dbService.GetStudents().ToList().OrderBy(x => x.LastName));
                case "IndexNumber":
                    return Ok(_dbService.GetStudents().ToList().OrderBy(x => x.IndexNumber));
                case "IdStudent":
                    return Ok(_dbService.GetStudents().ToList().OrderBy(x => x.IdStudent));
                case null:
                    return Ok(_dbService.GetStudents());
                default:
                    return NotFound("Invalid field");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            var st = new Student();

            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18731;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = "SELECT * FROM Student WHERE Student.IndexNumber=@id";
                com.Parameters.AddWithValue("id", "s" + id);

                client.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.BirthDate = dr["BirthDate"].ToString();
                    st.IdEnrollment = Int32.Parse(dr["IdEnrollment"].ToString());
                }
            }

            return Ok(st);
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            _dbService.CreateStudent(student);
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(Student student, int id)
        {
            var response = DeleteStudent(id);
            CreateStudent(student);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            try
            {
                return Ok(_dbService.RemoveStudent(id));
            }
            catch (InvalidOperationException)
            {
                return NotFound(NoIDFoundResponse);
            }
        }
    }
}