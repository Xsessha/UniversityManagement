namespace UniversityManagement.Web.ViewModels;

public class GroupViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int FacultyId { get; set; }

    public string? FacultyName { get; set; }

    public int StudentsCount { get; set; }

    public List<StudentViewModel> Students { get; set; } = new();
}