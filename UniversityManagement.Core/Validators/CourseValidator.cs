using UniversityManagement.Core.DTO;

namespace UniversityManagement.Core.Validators;

public class CourseValidator
{
    public bool Validate(CourseDTO course)
    {
        if (string.IsNullOrWhiteSpace(course.Name))
            return false;

        if (course.Credits <= 0)
            return false;

        return true;
    }
}