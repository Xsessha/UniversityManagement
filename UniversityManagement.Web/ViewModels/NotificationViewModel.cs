namespace UniversityManagement.Web.ViewModels;

public class NotificationViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsRead { get; set; }

    public string TimeAgo
    {
        get
        {
            var span = DateTime.Now - CreatedAt;

            if (span.TotalMinutes < 60)
                return $"{(int)span.TotalMinutes} min ago";

            if (span.TotalHours < 24)
                return $"{(int)span.TotalHours} h ago";

            return $"{(int)span.TotalDays} d ago";
        }
    }
}