using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CW_3_v2.Models;

namespace CW_3_v2.Services
{
    public interface IStudentsDbService
    {
        public bool IsStudentNumberUnique(int studentNumber);
        public bool StudiesAvailable (string studiesName);
        public void CreateEnrollment (EnrollmentPost student);
        public Enrollment ReturnedEnrollmentResult(EnrollmentPost student);
        public bool ConatinsEnrollment(PromotePost enrollmentPost);
        public bool ExecuteDatabaseProcedurePromote(PromotePost promotePost);
        public Enrollment PromotedEnrollmentStudent(PromotePost promotePost);
    }
}
