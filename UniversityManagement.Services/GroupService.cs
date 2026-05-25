using UniversityManagement.Core.Models;

namespace UniversityManagement.Services;

public class GroupService
{
    private readonly List<Group> _groups = new();

    public void Add(string name)
    {
        _groups.Add(new Group
        {
            Id = _groups.Count + 1,
            Name = name
        });
    }

    public List<Group> GetAll() => _groups;
}