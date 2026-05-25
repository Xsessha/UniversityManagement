using UniversityManagement.Core.Models;

namespace UniversityManagement.Services;

public class ReportService
{
    public Report GenerateStudentReport(
        Student student)
    {
        return new Report
        {
            Title = $"Report: {student.FullName}",
            GeneratedAt = DateTime.Now,
            Content =
                $"Student rating: {student.Rating}"
        };
    }
}