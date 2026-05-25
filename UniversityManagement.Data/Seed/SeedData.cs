using Microsoft.EntityFrameworkCore;
using UniversityManagement.Core.Enums;
using UniversityManagement.Core.Models;
using UniversityManagement.Data.Context;

namespace UniversityManagement.Data.Seed;

public static class SeedData
{
    public static async Task SeedAsync(UniversityDbContext context)
    {
        await SeedFacultiesAndGroupsAsync(context);
        await SeedTeachersAsync(context);
        await SeedCoursesAsync(context);
        await SeedStudentsAsync(context);
        await SeedLessonsAsync(context);
        await SeedAttendanceAsync(context);
        await SeedRatingsAsync(context);
        await SeedReportsAsync(context);
    }

    private static async Task SeedFacultiesAndGroupsAsync(UniversityDbContext context)
    {
        if (await context.Groups.CountAsync() >= 5)
        {
            return;
        }

        var faculties = await context.Faculties.OrderBy(f => f.Id).ToListAsync();

        if (faculties.Count == 0)
        {
            var computerScience = new Faculty
            {
                Name = "Faculty of Computer Science",
                Dean = "Dr. Hannah Lewis"
            };

            var business = new Faculty
            {
                Name = "Faculty of Business and Engineering",
                Dean = "Prof. Marcus Green"
            };

            context.Faculties.AddRange(computerScience, business);
            await context.SaveChangesAsync();
            faculties = await context.Faculties.OrderBy(f => f.Id).ToListAsync();
        }

        var groupData = new[]
        {
            "CS-221",
            "CS-231",
            "DS-241",
            "BA-221",
            "EN-231"
        };

        var existingNames = await context.Groups.Select(g => g.Name).ToListAsync();
        var groupsToAdd = groupData
            .Where(name => !existingNames.Contains(name))
            .Take(5 - await context.Groups.CountAsync())
            .Select((name, index) => new Group
            {
                Name = name,
                Faculty = faculties[Math.Min(index % faculties.Count, faculties.Count - 1)]
            })
            .ToList();

        context.Groups.AddRange(groupsToAdd);
        await context.SaveChangesAsync();
    }

    private static async Task SeedTeachersAsync(UniversityDbContext context)
    {
        var existingCount = await context.Teachers.CountAsync();

        if (existingCount >= 10)
        {
            return;
        }

        var teachers = new List<Teacher>
        {
            new() { FirstName = "Hannah", LastName = "Lewis", Department = "Computer Science", Email = "hannah.lewis@lms.edu" },
            new() { FirstName = "Marcus", LastName = "Green", Department = "Mathematics", Email = "marcus.green@lms.edu" },
            new() { FirstName = "Elena", LastName = "Foster", Department = "Business Administration", Email = "elena.foster@lms.edu" },
            new() { FirstName = "Victor", LastName = "Chen", Department = "Engineering", Email = "victor.chen@lms.edu" },
            new() { FirstName = "Amelia", LastName = "Wright", Department = "Data Science", Email = "amelia.wright@lms.edu" },
            new() { FirstName = "Samuel", LastName = "Reed", Department = "Physics", Email = "samuel.reed@lms.edu" },
            new() { FirstName = "Laura", LastName = "Morgan", Department = "Psychology", Email = "laura.morgan@lms.edu" },
            new() { FirstName = "David", LastName = "Kim", Department = "Economics", Email = "david.kim@lms.edu" },
            new() { FirstName = "Natalie", LastName = "Adams", Department = "Information Systems", Email = "natalie.adams@lms.edu" },
            new() { FirstName = "Robert", LastName = "Hughes", Department = "Cybersecurity", Email = "robert.hughes@lms.edu" }
        };

        var existingEmails = await context.Teachers.Select(t => t.Email).ToListAsync();
        var teachersToAdd = teachers
            .Where(t => !existingEmails.Contains(t.Email))
            .Take(10 - existingCount)
            .ToList();

        context.Teachers.AddRange(teachersToAdd);
        await context.SaveChangesAsync();
    }

    private static async Task SeedCoursesAsync(UniversityDbContext context)
    {
        var existingCount = await context.Courses.CountAsync();

        if (existingCount >= 10)
        {
            return;
        }

        var teachers = await context.Teachers.OrderBy(t => t.Id).Take(10).ToListAsync();
        var groups = await context.Groups.OrderBy(g => g.Id).Take(5).ToListAsync();

        if (teachers.Count < 10 || groups.Count == 0)
        {
            return;
        }

        var courseData = new[]
        {
            ("Introduction to Programming", "Programming basics, control flow, and problem solving.", 5),
            ("Calculus I", "Limits, derivatives, and applications of differential calculus.", 4),
            ("Principles of Management", "Core management concepts and organizational decision-making.", 3),
            ("Engineering Mechanics", "Statics, dynamics, and mechanical system analysis.", 5),
            ("Data Analytics Fundamentals", "Data preparation, visualization, and analytical thinking.", 4),
            ("General Physics", "Mechanics, energy, waves, and introductory thermodynamics.", 4),
            ("Cognitive Psychology", "Human cognition, perception, memory, and learning.", 3),
            ("Microeconomics", "Markets, consumer behavior, and firm decision-making.", 3),
            ("Database Systems", "Relational modeling, SQL, normalization, and transactions.", 5),
            ("Network Security", "Secure network design, threats, cryptography, and defense.", 4)
        };

        var existingNames = await context.Courses.Select(c => c.Name).ToListAsync();
        var courses = courseData
            .Where(course => !existingNames.Contains(course.Item1))
            .Take(10 - existingCount)
            .Select((course, index) =>
        {
            var entity = new Course
            {
                Name = course.Item1,
                Description = course.Item2,
                Credits = course.Item3,
                Teacher = teachers[index]
            };

            entity.Groups.Add(groups[index % groups.Count]);
            return entity;
        }).ToList();

        context.Courses.AddRange(courses);
        await context.SaveChangesAsync();
    }

    private static async Task SeedStudentsAsync(UniversityDbContext context)
    {
        var existingCount = await context.Students.CountAsync();

        if (existingCount >= 10)
        {
            return;
        }

        var groups = await context.Groups.OrderBy(g => g.Id).Take(5).ToListAsync();

        if (groups.Count == 0)
        {
            return;
        }

        var students = new List<Student>
        {
            new() { FirstName = "Olivia", LastName = "Bennett", Email = "olivia.bennett@lms.edu", BirthDate = new DateTime(2004, 3, 14), Status = StudentStatus.Active, Rating = 92, Group = groups[0] },
            new() { FirstName = "Ethan", LastName = "Carter", Email = "ethan.carter@lms.edu", BirthDate = new DateTime(2003, 8, 21), Status = StudentStatus.Active, Rating = 78, Group = groups[1] },
            new() { FirstName = "Sophia", LastName = "Mitchell", Email = "sophia.mitchell@lms.edu", BirthDate = new DateTime(2005, 1, 9), Status = StudentStatus.Active, Rating = 88, Group = groups[2] },
            new() { FirstName = "Daniel", LastName = "Brooks", Email = "daniel.brooks@lms.edu", BirthDate = new DateTime(2004, 6, 27), Status = StudentStatus.Suspended, Rating = 64, Group = groups[3] },
            new() { FirstName = "Mia", LastName = "Anderson", Email = "mia.anderson@lms.edu", BirthDate = new DateTime(2006, 2, 18), Status = StudentStatus.Active, Rating = 95, Group = groups[4] },
            new() { FirstName = "Noah", LastName = "Phillips", Email = "noah.phillips@lms.edu", BirthDate = new DateTime(2003, 11, 5), Status = StudentStatus.Active, Rating = 81, Group = groups[0] },
            new() { FirstName = "Ava", LastName = "Thompson", Email = "ava.thompson@lms.edu", BirthDate = new DateTime(2005, 4, 30), Status = StudentStatus.Active, Rating = 86, Group = groups[1] },
            new() { FirstName = "Lucas", LastName = "Rivera", Email = "lucas.rivera@lms.edu", BirthDate = new DateTime(2006, 7, 12), Status = StudentStatus.Active, Rating = 73, Group = groups[2] },
            new() { FirstName = "Emma", LastName = "Collins", Email = "emma.collins@lms.edu", BirthDate = new DateTime(2004, 9, 3), Status = StudentStatus.Suspended, Rating = 90, Group = groups[3] },
            new() { FirstName = "James", LastName = "Wilson", Email = "james.wilson@lms.edu", BirthDate = new DateTime(2003, 12, 16), Status = StudentStatus.Active, Rating = 84, Group = groups[4] }
        };

        var existingEmails = await context.Students.Select(s => s.Email).ToListAsync();
        var studentsToAdd = students
            .Where(s => !existingEmails.Contains(s.Email))
            .Take(10 - existingCount)
            .ToList();

        context.Students.AddRange(studentsToAdd);
        await context.SaveChangesAsync();
    }

    private static async Task SeedLessonsAsync(UniversityDbContext context)
    {
        var existingCount = await context.Lessons.CountAsync();

        if (existingCount >= 10)
        {
            return;
        }

        var courses = await context.Courses.OrderBy(c => c.Id).Take(10).ToListAsync();

        if (courses.Count < 10)
        {
            return;
        }

        var lessonTypes = new[]
        {
            LessonType.Lecture,
            LessonType.Seminar,
            LessonType.Practice,
            LessonType.Laboratory,
            LessonType.Workshop,
            LessonType.Lecture,
            LessonType.Seminar,
            LessonType.Practice,
            LessonType.Laboratory,
            LessonType.Consultation
        };

        var today = DateTime.Today;
        var existingTopics = await context.Lessons.Select(l => l.Topic).ToListAsync();
        var lessons = courses
            .Where(course => !existingTopics.Contains(course.Name))
            .Take(10 - existingCount)
            .Select((course, index) => new Lesson
            {
                Topic = course.Name,
                Course = course,
                Type = lessonTypes[index],
                Date = today.AddDays(-(24 - (index * 2))).Date.AddHours(9 + (index % 5))
            })
            .ToList();

        context.Lessons.AddRange(lessons);
        await context.SaveChangesAsync();
    }

    private static async Task SeedAttendanceAsync(UniversityDbContext context)
    {
        var existingCount = await context.Attendances.CountAsync();

        if (existingCount >= 10)
        {
            return;
        }

        var students = await context.Students.OrderBy(s => s.Id).Take(10).ToListAsync();
        var lessons = await context.Lessons.OrderBy(l => l.Id).Take(10).ToListAsync();

        if (students.Count < 10 || lessons.Count < 10)
        {
            return;
        }

        var statuses = new[]
        {
            AttendanceStatus.Present,
            AttendanceStatus.Late,
            AttendanceStatus.Present,
            AttendanceStatus.Absent,
            AttendanceStatus.Present,
            AttendanceStatus.Present,
            AttendanceStatus.Late,
            AttendanceStatus.Present,
            AttendanceStatus.Absent,
            AttendanceStatus.Present
        };

        var attendance = students.Skip(existingCount).Take(10 - existingCount).Select((student, index) =>
        {
            var lessonIndex = existingCount + index;

            return new Attendance
            {
            Student = student,
            Lesson = lessons[lessonIndex],
            Date = lessons[lessonIndex].Date.Date,
            Status = statuses[lessonIndex]
            };
        }).ToList();

        context.Attendances.AddRange(attendance);
        await context.SaveChangesAsync();
    }

    private static async Task SeedRatingsAsync(UniversityDbContext context)
    {
        var existingCount = await context.Grades.CountAsync();

        if (existingCount >= 10)
        {
            return;
        }

        var students = await context.Students.OrderBy(s => s.Id).Take(10).ToListAsync();
        var courses = await context.Courses.OrderBy(c => c.Id).Take(10).ToListAsync();

        if (students.Count < 10 || courses.Count < 10)
        {
            return;
        }

        var feedback = new[]
        {
            "Excellent coding fundamentals.",
            "Good progress, needs more practice.",
            "Strong participation and analysis.",
            "Needs improvement in problem solving.",
            "Outstanding data interpretation skills.",
            "Solid understanding of core concepts.",
            "Thoughtful written assignments.",
            "Good effort, improve exam accuracy.",
            "Very good database design work.",
            "Good lab work and security awareness."
        };

        var scores = new[] { 92, 78, 88, 64, 95, 81, 86, 73, 90, 84 };
        var today = DateTime.Today;
        var grades = students.Skip(existingCount).Take(10 - existingCount).Select((student, index) =>
        {
            var courseIndex = existingCount + index;

            return new Grade
            {
            Student = student,
            Course = courses[courseIndex],
            Value = scores[courseIndex],
            Comment = feedback[courseIndex],
            Date = today.AddDays(-(22 - (courseIndex * 2))).Date
            };
        }).ToList();

        context.Grades.AddRange(grades);
        await context.SaveChangesAsync();
    }

    private static async Task SeedReportsAsync(UniversityDbContext context)
    {
        var existingCount = await context.Reports.CountAsync();

        if (existingCount >= 10)
        {
            return;
        }

        var teachers = await context.Teachers.OrderBy(t => t.Id).Take(10).ToListAsync();

        if (teachers.Count < 10)
        {
            return;
        }

        var reportData = new[]
        {
            ("Programming Course Progress", "Monthly performance report for programming students."),
            ("Calculus Attendance Review", "Attendance and participation review for Calculus I."),
            ("Management Assessment Summary", "Assessment summary for management coursework."),
            ("Engineering Lab Report", "Lab completion and performance overview."),
            ("Data Analytics Performance", "Student progress report in analytics assignments."),
            ("Physics Midterm Overview", "Midterm performance and attendance report."),
            ("Psychology Seminar Summary", "Seminar participation and writing evaluation."),
            ("Economics Class Review", "Course engagement and quiz performance report."),
            ("Database Systems Review", "Database project and assessment summary."),
            ("Network Security Report", "End of module security lab performance report.")
        };

        var today = DateTime.Today;
        var existingTitles = await context.Reports.Select(r => r.Title).ToListAsync();
        var reports = reportData
            .Where(report => !existingTitles.Contains(report.Item1))
            .Take(10 - existingCount)
            .Select((report, index) =>
        {
            var teacherIndex = existingCount + index;

            return new Report
            {
                Title = report.Item1,
                Description = report.Item2,
                Content = report.Item2,
                GeneratedBy = teachers[teacherIndex].Id.ToString(),
                ReportType = "Academic Performance",
                FilePath = $"/reports/{report.Item1.ToLowerInvariant().Replace(' ', '-')}.pdf",
                CreatedAt = today.AddDays(-(23 - (teacherIndex * 2))).Date
            };
        }).ToList();

        context.Reports.AddRange(reports);
        await context.SaveChangesAsync();
    }
}
