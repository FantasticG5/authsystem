using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Dtos;

public class UpdateProfileRequest
{
    [Required, MinLength(2), MaxLength(50)]
    public string Firstname { get; set; } = null!;

    [Required, MinLength(2), MaxLength(50)]
    public string Lastname { get; set; } = null!;

    [Phone]
    public string? PhoneNumber { get; set; }
}