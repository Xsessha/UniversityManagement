using UniversityManagement.Core.Models;

namespace UniversityManagement.Patterns.Visitor;

public interface IVisitor
{
    void Visit(Student student);
    void Visit(Group group);
    void Visit(Faculty faculty);
}