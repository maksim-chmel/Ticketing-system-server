using AdminPanelBack.DTO;

namespace AdminPanelBack.Services.User;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsers(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Models.User?> ManageComment(long userId, string comment, CancellationToken cancellationToken = default);
    Task RegistrationNewUser(UserDto userDto, CancellationToken cancellationToken = default);
    Task<bool> IsUserExists(long userId, CancellationToken cancellationToken = default);
    Task<List<long>> GetAllUsersIds(CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserById(long userId, CancellationToken cancellationToken = default);
}
