using CW_3_v2.Models;
using System.Collections.Generic;

namespace CW_3_v2.DAL
{
    public interface IDbService
    {
        public IEnumerable<Student> GetStudents();
        public void CreateStudent(Student student);
        public IEnumerable<Student> UpdateStudent(Student student, int id);
        public IEnumerable<Student> RemoveStudent(int id);

    }
}
