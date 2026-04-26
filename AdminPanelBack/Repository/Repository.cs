using AdminPanelBack.DB;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Repository;

public class Repository<T>(AppDbContext context) : IRepository<T>
    where T : class
{
    public virtual async Task<T?> FindAsyncById(int id) =>
        await context.Set<T>().FindAsync(id);
    public virtual async Task<T?> FindAsyncById(long id) =>
        await context.Set<T>().FindAsync(id);
    public virtual async Task<List<T>> GetAllAsync() =>
        await context.Set<T>().ToListAsync();

    public virtual async Task AddAsync(T entity) =>
        await context.Set<T>().AddAsync(entity);

    public virtual void Update(T entity) =>
        context.Set<T>().Update(entity);

    public virtual void Remove(T entity) =>
        context.Set<T>().Remove(entity);
}