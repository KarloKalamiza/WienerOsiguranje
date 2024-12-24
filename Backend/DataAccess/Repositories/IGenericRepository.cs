namespace Backend.DataAccess.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> Get();
    Task<T> Find(int id);
    Task<T> Add(T model);
    Task<T> Update(T model);
    Task<int> Remove(int id);

}
