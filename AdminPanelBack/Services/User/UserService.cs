using AdminPanelBack.DTO;
using AdminPanelBack.Repository;
using AutoMapper;

namespace AdminPanelBack.Services.User;

public class UserService(IUserRepository repository,IMapper mapper,ILogger<UserService> logger) : IUserService
{
    public async Task<List<UserDto>> GetAllUsers()
    {
        var users =  await repository.GetAllAsync();
        return mapper.Map<List<UserDto>>(users);
    }
    public async Task<List<long>> GetAllUsersIds()
    {
        var users = await repository.GetAllAsync();
        
        return users.Select(user => user.UserId).ToList();
    }
    
    public async Task<Models.User?> ManageComment(long userId, string comment)
    {
        var user = await repository.FindAsyncById(userId);
        if (user == null) return null;
        user.Comments =  comment;
        await repository.SaveChangesAsync();
        return user;
    }

    public async Task<bool> RegistrationNewUser(UserDto userDto)
    {
       
        logger.LogDebug("Starting registration/update for UserID: {UserId}", userDto.UserId);

        var user = await repository.FindAsyncById(userDto.UserId);
    
        if (user != null) 
        {
            
            mapper.Map(userDto, user);
            await repository.SaveChangesAsync();
        
            logger.LogInformation("Existing user {UserId} successfully updated", userDto.UserId);
            return true; 
        }
    
       
        var newUser = mapper.Map<Models.User>(userDto);
    
        try 
        {
            await repository.AddAsync(newUser);
            await repository.SaveChangesAsync();
            
            logger.LogInformation("New user registered successfully. Assigned ID: {UserId}", newUser.UserId);
            return true;
        }
        catch (Exception ex)
        {
           
            logger.LogError(ex, "Failed to register new user {UserId}", userDto.UserId);
            throw;
        }
    }

    public async Task<bool> IsUserExists(long userId)
    {
        var user = await repository.FindAsyncById(userId);
        return user != null;
    }
    
   
}