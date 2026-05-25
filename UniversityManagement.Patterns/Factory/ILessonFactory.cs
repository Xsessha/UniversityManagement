using UniversityManagement.Core.Models;

namespace UniversityManagement.Patterns.Factory;

public interface ILessonFactory
{
    Lesson CreateLesson();
}