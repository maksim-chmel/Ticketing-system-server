using AdminPanelBack.DB;
using AdminPanelBack.DTO;
using AdminPanelBack.Repository;
using AutoMapper;

namespace AdminPanelBack.Services.User;

public class UserService(IUserRepository repository,IMapper mapper,ILogger<UserService> logger, AppDbContext context) : IUserService
{
    public async Task<List<UserDto>> GetAllUsers(int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;
        var users = await repository.GetUsersPageAsync(skip, pageSize);
        return mapper.Map<List<UserDto>>(users);
    }
    public async Task<List<long>> GetAllUsersIds()
    {
        return await repository.GetAllUserIdsAsync();
    }
    
    public async Task<Models.User?> ManageComment(long userId, string comment)
    {
        var user = await repository.FindAsyncById(userId);
        if (user == null) return null;
        user.Comments =  comment;
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> RegistrationNewUser(UserDto userDto)
    {
        logger.LogDebug("Starting registration/update for UserID: {UserId}", userDto.UserId);

        var user = await repository.FindAsyncById(userDto.UserId);
    
        if (user != null) 
        {
            mapper.Map(userDto, user);
            await context.SaveChangesAsync();
            logger.LogInformation("Existing user {UserId} successfully updated", userDto.UserId);
            return true; 
        }
        var newUser = mapper.Map<Models.User>(userDto);

        await repository.AddAsync(newUser);
        await context.SaveChangesAsync();
        logger.LogInformation("New user registered successfully. Assigned ID: {UserId}", newUser.UserId);
        return true;
    }

    public async Task<bool> IsUserExists(long userId)
    {
        var user = await repository.FindAsyncById(userId);
        return user != null;
    }

    public async Task<UserDto?> GetUserById(long userId)
    {
        var user = await repository.FindAsyncById(userId);
        return user == null ? null : mapper.Map<UserDto>(user);
    }
}
