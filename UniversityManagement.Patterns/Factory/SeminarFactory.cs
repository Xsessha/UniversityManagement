using UniversityManagement.Core.Models;

namespace UniversityManagement.Patterns.Factory;

public class SeminarFactory : ILessonFactory
{
    public Lesson CreateLesson()
    {
            return new Lesson { Topic = "Seminar", Type = Core.Enums.LessonType.Seminar };
    }
}