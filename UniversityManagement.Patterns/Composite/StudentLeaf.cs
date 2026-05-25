namespace UniversityManagement.Patterns.Composite;

public class StudentLeaf : IUniversityComponent
{
    public string Name { get; set; }

    public StudentLeaf(string name)
    {
        Name = name;
    }

    public void Display(int depth = 0)
    {
        Console.WriteLine(new string('-', depth) + " Student: " + Name);
    }
}