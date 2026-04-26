using AdminPanelBack.Models;

namespace AdminPanelBack.Repository;

public interface IUserRepository : IRepository<User>
{
    Task<List<long>> GetAllUserIdsAsync();
    Task<List<User>> GetUsersPageAsync(int skip, int take);
}
