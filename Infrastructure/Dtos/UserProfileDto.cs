namespace Infrastructure.Dtos;

public record UserProfileDto
{
    public string Id { get; init; } = null!;
    public string Firstname { get; init; } = null!;
    public string Lastname { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? PhoneNumber { get; init; }
}