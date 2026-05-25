using UniversityManagement.Core.Models;

namespace UniversityManagement.Patterns.Composite;

public class FacultyComposite : IUniversityComponent
{
    public string Name { get; set; }

    public List<IUniversityComponent> Children { get; set; } = new();

    public FacultyComposite(string name)
    {
        Name = name;
    }

    public void Add(IUniversityComponent component)
    {
        Children.Add(component);
    }

    public void Display(int depth = 0)
    {
        Console.WriteLine(new string('-', depth) + " Faculty: " + Name);

        foreach (var child in Children)
            child.Display(depth + 2);
    }
}