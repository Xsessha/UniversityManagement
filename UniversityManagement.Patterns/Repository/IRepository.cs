namespace UniversityManagement.Patterns.Repository;

public interface IRepository<T>
{
    void Add(T item);
    List<T> GetAll();
}