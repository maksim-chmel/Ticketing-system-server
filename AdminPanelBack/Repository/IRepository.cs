namespace AdminPanelBack.Repository;

public interface IRepository<T, TKey> where T : class
{
    Task<T?> FindAsyncById(TKey id, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
}
