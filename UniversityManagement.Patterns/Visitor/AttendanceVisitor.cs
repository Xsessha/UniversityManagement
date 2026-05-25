using UniversityManagement.Core.Models;

namespace UniversityManagement.Patterns.Visitor;

public class AttendanceVisitor : IVisitor
{
    public void Visit(Student student)
    {
        Console.WriteLine($"{student.FirstName} {student.LastName} attendance checked");
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