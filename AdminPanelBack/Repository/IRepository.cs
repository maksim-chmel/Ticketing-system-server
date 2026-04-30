namespace AdminPanelBack.Repository;

public interface IRepository<T> where T : class
{
    Task<T?> FindAsyncById(int id, CancellationToken cancellationToken = default);
    Task<T?> FindAsyncById(long id, CancellationToken cancellationToken = default);
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
}
