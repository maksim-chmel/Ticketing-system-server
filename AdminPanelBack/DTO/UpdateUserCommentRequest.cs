using System.ComponentModel.DataAnnotations;

namespace AdminPanelBack.DTO;

public sealed class UpdateUserCommentRequest
{
    [Required]
    [MaxLength(2000)]
    public required string Comment { get; set; }
}

