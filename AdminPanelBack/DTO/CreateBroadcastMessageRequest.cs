using System.ComponentModel.DataAnnotations;

namespace AdminPanelBack.DTO;

public sealed class CreateBroadcastMessageRequest
{ 
    [Required]
    [MaxLength(4000)]
    public required string Message { get; set; }
}