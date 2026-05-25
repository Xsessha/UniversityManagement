using UniversityManagement.Core.Models;

namespace UniversityManagement.Services;

public class ReportService
{
    public void GenerateStudentReport(Student student)
    {
        Console.WriteLine($"Report for {student.FirstName} {student.LastName}, Rating: {student.Rating}");
    }
}