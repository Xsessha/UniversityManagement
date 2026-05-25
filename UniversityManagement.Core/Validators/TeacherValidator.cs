using UniversityManagement.Core.DTO;

namespace UniversityManagement.Core.Validators;

public class TeacherValidator
{
    public bool Validate(TeacherDTO teacher)
    {
        if (string.IsNullOrWhiteSpace(teacher.FullName))
            return false;

        if (!teacher.Email.Contains("@"))
            return false;

        if (string.IsNullOrWhiteSpace(teacher.Department))
            return false;

        return true;
    }
}