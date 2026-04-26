namespace AdminPanelBack.Repository;

public interface IRepository<T> where T : class
{
    Task<T?> FindAsyncById(int id);
    Task<T?> FindAsyncById(long id);
    Task<List<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}