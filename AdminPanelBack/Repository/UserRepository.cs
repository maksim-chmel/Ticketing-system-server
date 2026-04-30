using AdminPanelBack.DB;
using AdminPanelBack.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelBack.Repository;

public class UserRepository(AppDbContext context) : Repository<User>(context)
    , IUserRepository
{
    private readonly AppDbContext _context = context;

    public Task<List<long>> GetAllUserIdsAsync(CancellationToken cancellationToken = default) =>
        _context.Clients
            .AsNoTracking()
            .Select(u => u.UserId)
            .ToListAsync(cancellationToken);

    public Task<List<User>> GetUsersPageAsync(int skip, int take, CancellationToken cancellationToken = default) =>
        _context.Clients
            .AsNoTracking()
            .OrderBy(u => u.UserId)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
}
