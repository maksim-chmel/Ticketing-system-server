using AdminPanelBack.DB;

namespace AdminPanelBack.Repository;

public abstract class Repository<T, TKey>(AppDbContext context) : IRepository<T, TKey>
    where T : class
{
    protected AppDbContext Context { get; } = context;

    public virtual async Task<T?> FindAsyncById(TKey id, CancellationToken cancellationToken = default) =>
        await Context.Set<T>().FindAsync([id], cancellationToken);

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await Context.Set<T>().AddAsync(entity, cancellationToken);

    public virtual void Update(T entity) =>
        Context.Set<T>().Update(entity);

    public virtual void Remove(T entity) =>
        Context.Set<T>().Remove(entity);
}
