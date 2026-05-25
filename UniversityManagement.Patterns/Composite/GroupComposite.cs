namespace UniversityManagement.Patterns.Composite;

public class GroupComposite : IUniversityComponent
{
    public string Name { get; set; }

    public List<IUniversityComponent> Children { get; set; } = new();

    public GroupComposite(string name)
    {
        Name = name;
    }

    public void Add(IUniversityComponent component)
    {
        Children.Add(component);
    }

    public void Display(int depth = 0)
    {
        Console.WriteLine(new string('-', depth) + " Group: " + Name);

        foreach (var child in Children)
            child.Display(depth + 2);
    }
}