namespace UniversityManagement.Web.ViewModels;

public class NotificationViewModel
{
    public int Id { get; set; }
    public string Message { get; set; } = "";

    public string? UserName { get; set; }
    public DateTime CreatedAt { get; set; }
}