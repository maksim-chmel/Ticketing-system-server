using Microsoft.AspNetCore.Identity;

namespace AdminPanelBack.Models;

public class Admin:IdentityUser
{
    public string Name { get; set; }     
}