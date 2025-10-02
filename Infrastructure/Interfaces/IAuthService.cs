using Infrastructure.Dtos;
using Infrastructure.Models;

namespace Infrastructure.Interfaces;

public interface IAuthService
{
    Task<ApiResult<object>> RegisterAsync(RegisterRequest request);
    Task<ApiResult<LoginResponse>> LoginAsync(LoginRequest request);
    Task<ApiResult<LoginResponse>> RefreshAsync(string refreshJwt);

    // valfritt att behålla dessa två; de kan vara no-op i JWT-världen
    Task<AuthServiceResult> LogoutAsync();
    Task<AuthServiceResult> ChangePasswordAsync(string userId, ChangePasswordRequest request);
}