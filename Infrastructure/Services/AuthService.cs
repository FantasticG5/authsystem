using Data.Entities;
using Infrastructure.Dtos;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class AuthService(UserManager<ApplicationUser> userManager,
                   SignInManager<ApplicationUser> signInManager) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;

    public async Task<AuthServiceResult> RegisterAsync(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            Firstname = request.Firstname,
            Lastname = request.Lastname,
            Email = request.Email,
            UserName = request.Email
        };

        if (request.Password != request.ConfirmedPassword)
        {
            return new AuthServiceResult
            {
                Succeeded = false,
                Error = "Passwords do not match",
                Message = "The password and confirmed password must be the same."
            };
        }

        var result = await _userManager.CreateAsync(user, request.Password);

        string errorMessage = string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));


        await _signInManager.SignInAsync(user, isPersistent: false);

        return result.Succeeded
            ? new AuthServiceResult { Succeeded = true }
            : new AuthServiceResult { Succeeded = false, Error = errorMessage, Message = "Failed to create user, debuga för mer info" };
    }

    public async Task<AuthServiceResult> LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        return new AuthServiceResult
        {
            Succeeded = true,
            Message = "Signed out"
        };
    }
}
