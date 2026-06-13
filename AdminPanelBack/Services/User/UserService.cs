using AdminPanelBack.DTO;
using AdminPanelBack.Exceptions;
using AdminPanelBack.Repository;
using AutoMapper;

namespace AdminPanelBack.Services.User;

public class UserService(IUserRepository repository, IMapper mapper, ILogger<UserService> logger, IUnitOfWork unitOfWork) : IUserService
{
    public async Task<PagedResult<UserDto>> GetAllUsers(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var skip = (page - 1) * pageSize;
        var usersTask = repository.GetUsersPageAsync(skip, pageSize, cancellationToken);
        var countTask = repository.GetCountAsync(cancellationToken);
        await Task.WhenAll(usersTask, countTask);
        return new PagedResult<UserDto> { Items = mapper.Map<List<UserDto>>(usersTask.Result), TotalCount = countTask.Result };
    }

    public async Task<List<long>> GetAllUsersIds(CancellationToken cancellationToken = default)
    {
        return await repository.GetAllUserIdsAsync(cancellationToken);
    }

    public async Task<UserDto?> ManageComment(long userId, string comment, CancellationToken cancellationToken = default)
    {
        var user = await repository.FindAsyncById(userId, cancellationToken);
        if (user == null)
        {
            logger.LogWarning("User not found when managing comment. UserId: {UserId}", userId);
            throw new NotFoundException($"User not found: {userId}");
        }
        user.Comments = comment;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Comment updated for user {UserId}", userId);
        return mapper.Map<UserDto>(user);
    }

    public async Task RegistrationNewUser(UserDto userDto, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting registration/update for UserID: {UserId}", userDto.UserId);

        var user = await repository.FindAsyncById(userDto.UserId, cancellationToken);

        if (user != null)
        {
            mapper.Map(userDto, user);
            logger.LogInformation("Existing user {UserId} successfully updated", userDto.UserId);
        }
        else
        {
            var newUser = mapper.Map<Models.User>(userDto);
            await repository.AddAsync(newUser, cancellationToken);
            logger.LogInformation("New user registered successfully. Assigned ID: {UserId}", newUser.UserId);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
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
