using UniversityManagement.Core.Models;

namespace UniversityManagement.Core.Interfaces;

public interface IVisitor
{
    void Visit(Student student);
    void Visit(Teacher teacher);
    void Visit(Course course);
    void Visit(Group group);
    void Visit(Faculty faculty);
}