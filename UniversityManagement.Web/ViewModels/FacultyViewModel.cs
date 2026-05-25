namespace UniversityManagement.Web.ViewModels;

public class FacultyViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public int GroupsCount { get; set; }
    public int StudentsCount { get; set; }
}