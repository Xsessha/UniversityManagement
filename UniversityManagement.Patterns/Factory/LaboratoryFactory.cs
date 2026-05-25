using UniversityManagement.Core.Models;

namespace UniversityManagement.Patterns.Factory;

public class LaboratoryFactory : ILessonFactory
{
    public Lesson CreateLesson()
    {
            return new Lesson { Topic = "Lab", Type = Core.Enums.LessonType.Laboratory };
    }
}