using AdminPanelBack.Models;

namespace AdminPanelBack.Repository;

public interface IUserRepository : IRepository<User>
{
    Task<List<long>> GetAllUserIdsAsync(CancellationToken cancellationToken = default);
    Task<List<User>> GetUsersPageAsync(int skip, int take, CancellationToken cancellationToken = default);
}
