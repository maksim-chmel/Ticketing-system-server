using AdminPanelBack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
namespace AdminPanelBack.Tests;

public class FakeUserManager : UserManager<Admin>
{
    public Admin? UserToReturn { get; set; }

    public FakeUserManager() : base(
        new Mock<IUserStore<Admin>>().Object,
        new Mock<IOptions<IdentityOptions>>().Object,
        new Mock<IPasswordHasher<Admin>>().Object,
        new IUserValidator<Admin>[0],
        new IPasswordValidator<Admin>[0],
        new Mock<ILookupNormalizer>().Object,
        new IdentityErrorDescriber(),
        new Mock<IServiceProvider>().Object,
        new Mock<ILogger<UserManager<Admin>>>().Object)
    { }

    public override Task<Admin?> FindByIdAsync(string userId)
        => Task.FromResult(UserToReturn);

    public override Task<Admin?> FindByNameAsync(string userName)
        => Task.FromResult(UserToReturn);
}