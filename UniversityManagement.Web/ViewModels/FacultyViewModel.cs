namespace UniversityManagement.Web.ViewModels;

public class FacultyViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int GroupsCount { get; set; }

    public int StudentsCount { get; set; }

    public double AverageRating { get; set; }

    public List<GroupViewModel> Groups { get; set; } = new();
}