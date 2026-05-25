namespace UniversityManagement.Web.ViewModels;

public class CourseViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Credits { get; set; }

    public string? TeacherName { get; set; }
}