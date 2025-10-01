using System.Security.Claims;
using Infrastructure.Dtos;
using Infrastructure.Models;

namespace Infrastructure.Interfaces;

public interface IAuthService
{
    Task<AuthServiceResult> RegisterAsync(RegisterRequest request);

    Task<AuthServiceResult> LogoutAsync();
    Task<AuthServiceResult> LoginAsync(LoginRequest request);

    Task<AuthServiceResult> ChangePasswordAsync(string userId, ChangePasswordRequest request);

    Task<AuthServiceResult<UserProfileDto>> GetMyProfileAsync(ClaimsPrincipal user);
    Task<AuthServiceResult<UserProfileDto>> UpdateMyProfileAsync(ClaimsPrincipal user, UpdateProfileRequest request);
}