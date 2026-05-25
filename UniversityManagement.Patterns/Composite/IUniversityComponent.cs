namespace UniversityManagement.Patterns.Composite;

public interface IUniversityComponent
{
    string Name { get; }
    void Display(int depth = 0);
}