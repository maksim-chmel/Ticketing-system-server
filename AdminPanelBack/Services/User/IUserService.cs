using AdminPanelBack.DTO;

namespace AdminPanelBack.Services.User;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsers(int page, int pageSize);
    Task<Models.User?> ManageComment(long userId, string comment);
    Task<bool> RegistrationNewUser(UserDto userDto);
    Task<bool> IsUserExists(long userId);
    Task<List<long>> GetAllUsersIds();
    Task<UserDto?> GetUserById(long userId);
}
