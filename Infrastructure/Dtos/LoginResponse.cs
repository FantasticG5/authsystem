using System;

namespace Infrastructure.Dtos;

public class LoginResponse
{
    public string AccessToken { get; set; } = "";
    public DateTime AccessExpiresUtc { get; set; }
    public string RefreshToken { get; set; } = "";
    public DateTime RefreshExpiresUtc { get; set; }
}
