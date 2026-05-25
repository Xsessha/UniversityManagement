namespace UniversityManagement.Patterns.Repository;

public class GenericRepository<T> : IRepository<T> where T : class
{
    private readonly List<T> _items = new();

    public void Add(T item)
    {
        _items.Add(item);
    }

    public List<T> GetAll()
    {
        return new List<T>(_items);
    }
}