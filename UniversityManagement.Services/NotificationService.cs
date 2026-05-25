namespace UniversityManagement.Services;

public class NotificationService
{
    public void Send(string message)
    {
        Console.WriteLine($"[NOTIFICATION]: {message}");
    }
}