using AdminPanelBack.DB;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Repository;

public abstract class Repository<T>(AppDbContext context) : IRepository<T>
    where T : class
{
    protected AppDbContext Context { get; } = context;

    public virtual async Task<T?> FindAsyncById(int id, CancellationToken cancellationToken = default) =>
        await Context.Set<T>().FindAsync([id], cancellationToken);

    public virtual async Task<T?> FindAsyncById(long id, CancellationToken cancellationToken = default) =>
        await Context.Set<T>().FindAsync([id], cancellationToken);

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await Context.Set<T>().AddAsync(entity, cancellationToken);

    public virtual void Update(T entity) =>
        Context.Set<T>().Update(entity);

    public virtual void Remove(T entity) =>
        Context.Set<T>().Remove(entity);
}
