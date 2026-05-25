using UniversityManagement.Core.Models;

namespace UniversityManagement.Patterns.Visitor;

public class RatingVisitor : IVisitor
{
    public void Visit(Student student)
    {
        student.Rating += 10;
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