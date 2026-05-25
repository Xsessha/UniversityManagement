namespace UniversityManagement.Patterns.Factory;

public static class LessonFactoryProvider
{
    public static ILessonFactory GetFactory(string type)
    {
        return type switch
        {
            "lecture" => new LectureFactory(),
            "seminar" => new SeminarFactory(),
            "lab" => new LaboratoryFactory(),
            _ => new LectureFactory()
        };
    }
}