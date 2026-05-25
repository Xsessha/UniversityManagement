using UniversityManagement.Core.DTO;

namespace UniversityManagement.Core.Validators;

public class StudentValidator
{
    public bool Validate(StudentDTO student)
    {
        if (string.IsNullOrWhiteSpace(student.FullName))
            return false;

        if (!student.Email.Contains("@"))
            return false;

        return true;
    }
}