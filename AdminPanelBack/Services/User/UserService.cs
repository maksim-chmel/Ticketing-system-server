using AdminPanelBack.DTO;
using AdminPanelBack.Repository;
using AutoMapper;

namespace AdminPanelBack.Services.User;

public class UserService(IUserRepository repository,IMapper mapper) : IUserService
{
    public async Task<List<UserDto>> GetAllUsers()
    {
        var users =  await repository.GetAllAsync();
        return mapper.Map<List<UserDto>>(users);
    }
    
    public async Task<Models.User?> ManageComment(long userId, string comment)
    {
        var user = await repository.FindAsyncById(userId);
        if (user == null) return null;
        user.Comments =  comment;
        await repository.SaveChangesAsync();
        return user;
    }
}