namespace UniversityManagement.Patterns.Observer;

public class NotificationManager
{
    public void Send(string message)
    {
        Console.WriteLine("Notification: " + message);
    }
}