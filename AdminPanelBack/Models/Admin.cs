using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AdminPanelBack.Models;

public class Admin:IdentityUser
{
    [StringLength(100)]
    public string? Name { get; set; }     
}
