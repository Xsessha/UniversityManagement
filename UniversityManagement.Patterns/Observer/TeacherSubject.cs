namespace UniversityManagement.Patterns.Observer;

public class TeacherSubject
{
    private List<IObserver> observers = new();

    public void Attach(IObserver observer) => observers.Add(observer);

    public void Notify(string message)
    {
        foreach (var o in observers)
            o.Update(message);
    }
}