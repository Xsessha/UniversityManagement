namespace UniversityManagement.Web.ViewModels;

public class StudentViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Status { get; set; } = "";

    public string? GroupName { get; set; }
    public double Rating { get; set; }
}