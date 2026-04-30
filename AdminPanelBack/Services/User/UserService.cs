using AdminPanelBack.DB;
using AdminPanelBack.DTO;
using AdminPanelBack.Repository;
using AutoMapper;

namespace AdminPanelBack.Services.User;

public class UserService(IUserRepository repository,IMapper mapper,ILogger<UserService> logger, AppDbContext context) : IUserService
{
    public async Task<List<UserDto>> GetAllUsers(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var skip = (page - 1) * pageSize;
        var users = await repository.GetUsersPageAsync(skip, pageSize, cancellationToken);
        return mapper.Map<List<UserDto>>(users);
    }
    public async Task<List<long>> GetAllUsersIds(CancellationToken cancellationToken = default)
    {
        return await repository.GetAllUserIdsAsync(cancellationToken);
    }
    
    public async Task<Models.User?> ManageComment(long userId, string comment, CancellationToken cancellationToken = default)
    {
        var user = await repository.FindAsyncById(userId, cancellationToken);
        if (user == null) return null;
        user.Comments =  comment;
        await context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<bool> RegistrationNewUser(UserDto userDto, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting registration/update for UserID: {UserId}", userDto.UserId);

        var user = await repository.FindAsyncById(userDto.UserId, cancellationToken);
    
        if (user != null) 
        {
            mapper.Map(userDto, user);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Existing user {UserId} successfully updated", userDto.UserId);
            return true; 
        }
        var newUser = mapper.Map<Models.User>(userDto);

        await repository.AddAsync(newUser, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("New user registered successfully. Assigned ID: {UserId}", newUser.UserId);
        return true;
    }

    public async Task<bool> IsUserExists(long userId, CancellationToken cancellationToken = default)
    {
        var user = await repository.FindAsyncById(userId, cancellationToken);
        return user != null;
    }

    public async Task<UserDto?> GetUserById(long userId, CancellationToken cancellationToken = default)
    {
        var user = await repository.FindAsyncById(userId, cancellationToken);
        return user == null ? null : mapper.Map<UserDto>(user);
    }
}
