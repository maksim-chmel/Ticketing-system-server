using AdminPanelBack.Models;

namespace AdminPanelBack.Repository;

public interface IUserRepository : IRepository<User, long>
{
    Task<List<long>> GetAllUserIdsAsync(CancellationToken cancellationToken = default);
    Task<List<User>> GetUsersPageAsync(int skip, int take, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}
