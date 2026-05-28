using FluentAssertions;
using UniversityManagement.Patterns.Factory;
using Xunit;

namespace UniversityManagement.Tests;

public class FactoryTests
{
    [Fact]
    public void LectureFactory_Should_Create_Lesson_With_Lecture_Topic()
    {
        var factory = new LectureFactory();

        var lesson = factory.CreateLesson();

        Assert.NotNull(lesson);
        lesson.Topic.Should().Be("Lecture");
        lesson.Type.Should().Be(UniversityManagement.Core.Enums.LessonType.Lecture);
    }

    [Fact]
    public void LaboratoryFactory_Should_Create_Lesson_With_Laboratory_Topic()
    {
        var factory = new LaboratoryFactory();

        var lesson = factory.CreateLesson();

        Assert.NotNull(lesson);
        lesson.Topic.Should().Be("Lab"); 
        lesson.Type.Should().Be(UniversityManagement.Core.Enums.LessonType.Laboratory);
    }

    [Fact]
    public void SeminarFactory_Should_Create_Lesson_With_Seminar_Topic()
    {
        var factory = new SeminarFactory();

        var lesson = factory.CreateLesson();

        Assert.NotNull(lesson);
        lesson.Topic.Should().Be("Seminar");
        lesson.Type.Should().Be(UniversityManagement.Core.Enums.LessonType.Seminar);
    }

    [Fact]
public void LessonFactoryProvider_Should_Return_LectureFactory_For_Lecture_Type()
{
    var factory = LessonFactoryProvider.GetFactory("lecture"); // string, не enum

    Assert.NotNull(factory);
    factory.GetType().Should().Be(typeof(LectureFactory));
}

    [Fact]
    public void All_Factories_Should_Create_Different_LessonTypes()
    {
        var lectureLesson    = new LectureFactory().CreateLesson();
        var seminarLesson    = new SeminarFactory().CreateLesson();
        var laboratoryLesson = new LaboratoryFactory().CreateLesson();

        lectureLesson.Type.Should().NotBe(seminarLesson.Type);
        lectureLesson.Type.Should().NotBe(laboratoryLesson.Type);
        seminarLesson.Type.Should().NotBe(laboratoryLesson.Type);
    }
}