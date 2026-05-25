using UniversityManagement.Core.Models;

namespace UniversityManagement.Patterns.Visitor;

public class ReportVisitor : IVisitor
{
    public void Visit(Student student)
    {
        Console.WriteLine($"Report for {student.FirstName} {student.LastName}");
    }

    public void Visit(Group group)
    {
        foreach (var s in group.Students)
            Visit(s);
    }

    public void Visit(Faculty faculty)
    {
        foreach (var g in faculty.Groups)
            Visit(g);
    }
}