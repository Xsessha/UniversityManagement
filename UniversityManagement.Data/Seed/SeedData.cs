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
        await SeedCourseGroupsAsync(context);
        await SeedLessonsAsync(context);
        await SeedSchedulesAsync(context);
        await SeedAttendanceAsync(context);
        await SeedRatingsAsync(context);
        await SeedReportsAsync(context);
        await NormalizeReportGeneratorsAsync(context);
        await SeedNotificationsAsync(context);
    }

    private static async Task SeedFacultiesAndGroupsAsync(UniversityDbContext context)
    {
        var facultyData = new[]
        {
            ("Faculty of Computer Science", "Dr. Hannah Lewis"),
            ("Faculty of Business and Engineering", "Prof. Marcus Green"),
            ("Faculty of Data Science", "Dr. Amelia Wright"),
            ("Faculty of Natural Sciences", "Prof. Samuel Reed"),
            ("Faculty of Social Sciences", "Dr. Laura Morgan"),
            ("Faculty of Cybersecurity", "Prof. Robert Hughes")
        };

        var existingFacultyNames = await context.Faculties.Select(f => f.Name).ToListAsync();
        foreach (var faculty in facultyData.Where(f => !existingFacultyNames.Contains(f.Item1)))
        {
            context.Faculties.Add(new Faculty
            {
                Name = faculty.Item1,
                Dean = faculty.Item2
            });
        }

        await context.SaveChangesAsync();

        var faculties = await context.Faculties.OrderBy(f => f.Id).ToListAsync();
        if (faculties.Count == 0) return;

        var groupData = new[]
        {
            ("CS-221", "Faculty of Computer Science"),
            ("CS-231", "Faculty of Computer Science"),
            ("DS-241", "Faculty of Data Science"),
            ("BA-221", "Faculty of Business and Engineering"),
            ("EN-231", "Faculty of Business and Engineering"),
            ("NS-201", "Faculty of Natural Sciences"),
            ("PS-202", "Faculty of Social Sciences"),
            ("CY-301", "Faculty of Cybersecurity"),
            ("SE-302", "Faculty of Computer Science"),
            ("AI-401", "Faculty of Data Science")
        };
        var existingNames = await context.Groups.Select(g => g.Name).ToListAsync();

        foreach (var group in groupData.Where(g => !existingNames.Contains(g.Item1)))
        {
            var faculty = faculties.FirstOrDefault(f => f.Name == group.Item2) ?? faculties.First();
            context.Groups.Add(new Group
            {
                Name = group.Item1,
                Faculty = faculty
            });
        }

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

    private static async Task SeedCourseGroupsAsync(UniversityDbContext context)
    {
        var courses = await context.Courses
            .Include(c => c.Groups)
            .OrderBy(c => c.Id)
            .ToListAsync();
        var groups = await context.Groups.OrderBy(g => g.Id).ToListAsync();

        if (courses.Count == 0 || groups.Count == 0)
        {
            return;
        }

        for (var index = 0; index < courses.Count; index++)
        {
            var course = courses[index];
            var firstGroup = groups[index % groups.Count];
            var secondGroup = groups[(index + 1) % groups.Count];

            if (!course.Groups.Any(g => g.Id == firstGroup.Id))
            {
                course.Groups.Add(firstGroup);
            }

            if (!course.Groups.Any(g => g.Id == secondGroup.Id))
            {
                course.Groups.Add(secondGroup);
            }
        }

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
                GeneratedBy = $"{teachers[teacherIndex].FirstName} {teachers[teacherIndex].LastName}",
                ReportType = "Academic Performance",
                FilePath = $"/reports/{report.Item1.ToLowerInvariant().Replace(' ', '-')}.pdf",
                CreatedAt = today.AddDays(-(23 - (teacherIndex * 2))).Date
            };
        }).ToList();

        context.Reports.AddRange(reports);
        await context.SaveChangesAsync();
    }

    private static async Task SeedSchedulesAsync(UniversityDbContext context)
    {
        var existingLessonIds = await context.Schedules.Select(s => s.LessonId).ToListAsync();
        var lessons = await context.Lessons
            .Include(l => l.Course)
            .ThenInclude(c => c!.Groups)
            .OrderBy(l => l.Date)
            .Take(10)
            .ToListAsync();
        var groups = await context.Groups.OrderBy(g => g.Id).Take(10).ToListAsync();

        if (lessons.Count == 0 || groups.Count == 0)
        {
            return;
        }

        var schedules = lessons
            .Where(lesson => !existingLessonIds.Contains(lesson.Id))
            .Select((lesson, index) =>
            {
                var group = lesson.Course?.Groups.FirstOrDefault() ?? groups[index % groups.Count];
                return new Schedule
                {
                    GroupId = group.Id,
                    LessonId = lesson.Id,
                    DayOfWeek = lesson.Date.DayOfWeek.ToString(),
                    StartTime = lesson.Date.ToString("HH:mm"),
                    EndTime = lesson.Date.AddMinutes(90).ToString("HH:mm")
                };
            })
            .ToList();

        context.Schedules.AddRange(schedules);
        await context.SaveChangesAsync();
    }

    private static async Task SeedNotificationsAsync(UniversityDbContext context)
    {
        if (await context.Notifications.CountAsync() >= 8)
        {
            return;
        }

        var students = await context.Students.OrderBy(s => s.Id).Take(8).ToListAsync();
        if (students.Count == 0)
        {
            return;
        }

        var notificationData = new[]
        {
            ("Welcome to Lumina Campus", "Your account is active and connected to the university workspace."),
            ("Schedule updated", "Two lessons were moved to new time slots this week."),
            ("Attendance reminder", "Please review attendance records before Friday."),
            ("New course materials", "Teachers uploaded fresh materials for active courses."),
            ("Rating update", "Student ratings were recalculated after the latest grades."),
            ("Faculty meeting", "Faculty reports are ready for review."),
            ("Scholarship review", "Top student ratings are available in the Ratings module."),
            ("System notice", "The university dashboard was refreshed with current analytics.")
        };

        var existingTitles = await context.Notifications.Select(n => n.Title).ToListAsync();
        var today = DateTime.Now;
        var notifications = notificationData
            .Where(n => !existingTitles.Contains(n.Item1))
            .Select((n, index) => new Notification
            {
                Title = n.Item1,
                Message = n.Item2,
                StudentId = students[index % students.Count].Id,
                CreatedAt = today.AddHours(-index * 4),
                IsRead = index % 3 == 0
            })
            .ToList();

        context.Notifications.AddRange(notifications);
        await context.SaveChangesAsync();
    }

    private static async Task NormalizeReportGeneratorsAsync(UniversityDbContext context)
    {
        var teachers = await context.Teachers.ToDictionaryAsync(t => t.Id, t => $"{t.FirstName} {t.LastName}");
        var reports = await context.Reports.ToListAsync();
        var changed = false;

        foreach (var report in reports)
        {
            if (int.TryParse(report.GeneratedBy, out var teacherId) && teachers.TryGetValue(teacherId, out var teacherName))
            {
                report.GeneratedBy = teacherName;
                changed = true;
            }
        }

        if (changed)
        {
            await context.SaveChangesAsync();
        }
    }
}
