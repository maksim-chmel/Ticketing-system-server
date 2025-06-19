using System.ComponentModel.DataAnnotations;

namespace AdminPanelBack.Models;

public class Admin
{
    [Key]
    public long UserId { get; set; }               
    public string Username { get; set; }           
    public string PasswordHash { get; set; }      
}