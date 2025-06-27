using AdminPanelBack.DTO;
using AdminPanelBack.Models;
using AutoMapper;

namespace AdminPanelBack.Profiles;

public class UserProfile:Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();
    }
}