using UniversityManagement.Core.Models;

namespace UniversityManagement.Patterns.Visitor;

public class StatisticsVisitor : IVisitor
{
    public void Visit(Student student)
    {
        Console.WriteLine($"Student: {student.FirstName} {student.LastName} Rating: {student.Rating}");
    }

    public void Visit(Group group)
    {
        Console.WriteLine($"Group: {group.Name}");
        foreach (var s in group.Students)
            Visit(s);
    }

    public void Visit(Faculty faculty)
    {
        Console.WriteLine($"Faculty: {faculty.Name}");
        foreach (var g in faculty.Groups)
            Visit(g);
    }
}