using CW_3_v2.Models;
using System.Collections.Generic;
using System.Linq;

namespace CW_3_v2.DAL
{
    public class MockDbService : IDbService
    {
        private static IEnumerable<Student> _students;

        static MockDbService()
        {
            _students = new List<Student>
            {
                new Student{IdStudent=0, FirstName="Jan", LastName="Kowalski", IndexNumber = $"s{new System.Random().Next(1, 20000)}"},
                new Student{IdStudent=1, FirstName="Anna", LastName="Malewska", IndexNumber = $"s{new System.Random().Next(1, 20000)}"},
                new Student{IdStudent=2, FirstName="Andrzej", LastName="Andrzejewski", IndexNumber = $"s{new System.Random().Next(1, 20000)}"}
            };
        }

        public void CreateStudent(Student student)
        {
            List<Student> tmpList = new List<Student>();
            tmpList.Add(
                new Student
                {
                    IdStudent = (_students.ToList().Max<Student>(x => x.IdStudent) + 1),
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    IndexNumber = $"s{new System.Random().Next(1, 20000)}"
                }
            );

            _students.Concat(tmpList);
        }

        public IEnumerable<Student> GetStudents()
        {
            return _students;
        }

        public IEnumerable<Student> RemoveStudent(int id)
        {
            var listWithoutDeletedObject = _students.ToList();
            var response = _students.Where((x => x.IdStudent == id)).ToList();

            listWithoutDeletedObject.Remove(listWithoutDeletedObject.First<Student>(x => x.IdStudent == id));
            
            _students = listWithoutDeletedObject;

            return response;
        }

        public IEnumerable<Student> UpdateStudent(Student student, int id)
        {
            return null;
        }
    }
}
