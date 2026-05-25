namespace UniversityManagement.Web.ViewModels;

public class AttendanceViewModel
{
    public int Id { get; set; }

    public string StudentName { get; set; } = "";
    public string CourseName { get; set; } = "";

    public string Status { get; set; } = "";
    public DateTime Date { get; set; }
}



