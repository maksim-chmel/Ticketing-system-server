using AdminPanelBack.DB;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Repository;

public class Repository<T>(AppDbContext context) : IRepository<T>
    where T : class
{
    public virtual async Task<T?> FindAsyncById(int id, CancellationToken cancellationToken = default) =>
        await context.Set<T>().FindAsync([id], cancellationToken);

    public virtual async Task<T?> FindAsyncById(long id, CancellationToken cancellationToken = default) =>
        await context.Set<T>().FindAsync([id], cancellationToken);

    public virtual async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Set<T>().ToListAsync(cancellationToken);

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await context.Set<T>().AddAsync(entity, cancellationToken);

    public virtual void Update(T entity) =>
        context.Set<T>().Update(entity);

    public virtual void Remove(T entity) =>
        context.Set<T>().Remove(entity);
}
