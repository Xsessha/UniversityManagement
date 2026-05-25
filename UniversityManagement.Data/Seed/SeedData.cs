using UniversityManagement.Core.Models;
using UniversityManagement.Core.Enums;

namespace UniversityManagement.Data.Seed;

public static class SeedData
{
    public static List<Student> GetStudents()
    {
        return new List<Student>
        {
            new Student
            {
                Id = 1,
                FirstName = "Ivan",
                LastName = "Petrenko",
                Email = "ivan@gmail.com",
                Status = StudentStatus.Active,
                Rating = 85
            },
            new Student
            {
                Id = 2,
                FirstName = "Maria",
                LastName = "Shevchenko",
                Email = "maria@gmail.com",
                Status = StudentStatus.Active,
                Rating = 92
            }
        };
    }

    public static List<Teacher> GetTeachers()
    {
        return new List<Teacher>
        {
            new Teacher
            {
                Id = 1,
                FirstName = "Dr.",
                LastName = "Smith",
                Email = "smith@university.com",
                Department = "Computer Science"
            }
        };
    }

    public static List<Course> GetCourses(List<Teacher> teachers)
    {
        return new List<Course>
        {
            new Course
            {
                Id = 1,
                Name = "OOP",
                Credits = 5,
                Teacher = teachers.First()
            }
        };
    }
}