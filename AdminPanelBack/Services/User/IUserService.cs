using AdminPanelBack.DTO;

namespace AdminPanelBack.Services.User;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsers();
    Task<Models.User?> ManageComment(long userId, string comment);
}