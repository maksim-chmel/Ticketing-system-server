using AdminPanelBack.DTO;
using AdminPanelBack.Models;

namespace AdminPanelBack.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsers();
    Task<User?> ManageComment(long userId, string comment);
}