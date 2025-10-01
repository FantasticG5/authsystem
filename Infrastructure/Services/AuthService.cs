using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Data.Entities;
using Infrastructure.Dtos;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Infrastructure.option;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtOptions _opt;
    private readonly IJwtTokenService _jwtService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IOptions<JwtOptions> opt,
        IJwtTokenService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _opt = opt.Value;
        _jwtService = jwtService;
    }

    public async Task<ApiResult<object>> RegisterAsync(RegisterRequest req)
    {
        if (req.Password != req.ConfirmedPassword)
            return ApiResult<object>.Fail("Passwords do not match");

        var user = new ApplicationUser
        {
            Firstname = req.Firstname,
            Lastname = req.Lastname,
            Email = req.Email,
            UserName = req.Email
        };

        var result = await _userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
        {
            var msg = string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
            return ApiResult<object>.Fail(msg);
        }

        return ApiResult<object>.Ok(new { user.Id, user.Email });
    }

    public async Task<ApiResult<LoginResponse>> LoginAsync(LoginRequest req)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null) return ApiResult<LoginResponse>.Fail("Invalid credentials");

        var ok = await _userManager.CheckPasswordAsync(user, req.Password);
        if (!ok) return ApiResult<LoginResponse>.Fail("Invalid credentials");

        var roles = await _userManager.GetRolesAsync(user);
        var (access, aexp) = _jwtService.CreateAccessToken(user, roles);
        var (refresh, rexp) = _jwtService.CreateRefreshToken(user);

        return ApiResult<LoginResponse>.Ok(new LoginResponse
        {
            AccessToken = access,
            AccessExpiresUtc = aexp,
            RefreshToken = refresh,
            RefreshExpiresUtc = rexp
        });
    }

    public async Task<ApiResult<LoginResponse>> RefreshAsync(string refreshJwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var tvp = new TokenValidationParameters
        {
            ValidIssuer = _opt.Issuer,
            ValidAudience = _opt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_opt.RefreshKey)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true
        };

        ClaimsPrincipal principal;
        try { principal = handler.ValidateToken(refreshJwt, tvp, out _); }
        catch { return ApiResult<LoginResponse>.Fail("Invalid refresh token"); }

        if (principal.FindFirst("typ")?.Value != "refresh")
            return ApiResult<LoginResponse>.Fail("Invalid refresh token");

        var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return ApiResult<LoginResponse>.Fail("Invalid refresh token");

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return ApiResult<LoginResponse>.Fail("User not found");

        var roles = await _userManager.GetRolesAsync(user);
        var (access, aexp) = _jwtService.CreateAccessToken(user, roles);
        var (newRefresh, rexp) = _jwtService.CreateRefreshToken(user);

        return ApiResult<LoginResponse>.Ok(new LoginResponse
        {
            AccessToken = access,
            AccessExpiresUtc = aexp,
            RefreshToken = newRefresh,
            RefreshExpiresUtc = rexp
        });
    }

    public async Task<AuthServiceResult> LogoutAsync()
    {
        // JWT är stateless – ingen riktig “logout”; detta påverkar ev. cookie-sess. Behåll eller ta bort.
        await _signInManager.SignOutAsync();
        return new AuthServiceResult { Succeeded = true, Message = "Signed out" };
    }

    public async Task<AuthServiceResult> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        if (request.NewPassword != request.ConfirmNewPassword)
            return new AuthServiceResult { Succeeded = false, Error = "Passwords do not match." };

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return new AuthServiceResult { Succeeded = false, Error = "User not found." };

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            var errorMessage = string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
            return new AuthServiceResult { Succeeded = false, Error = errorMessage };
        }

        return new AuthServiceResult { Succeeded = true, Message = "Password changed successfully." };
    }
}
