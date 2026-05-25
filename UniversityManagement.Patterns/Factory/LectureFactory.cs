using UniversityManagement.Core.Models;

namespace UniversityManagement.Patterns.Factory;

public class LectureFactory : ILessonFactory
{
    public Lesson CreateLesson()
    {
            return new Lesson { Topic = "Lecture", Type = Core.Enums.LessonType.Lecture };
    }
}