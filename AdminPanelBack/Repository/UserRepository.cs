using AdminPanelBack.DB;
using AdminPanelBack.Models;

namespace AdminPanelBack.Repository;

public class UserRepository(AppDbContext context) : Repository<User>(context)
    , IUserRepository
{
}