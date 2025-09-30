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

        var emailExists = await ExistsEmailAsync(request.Email);
        
        if (!emailExists.Succeeded)
        {
            return new AuthServiceResult
            {
                Succeeded = false,
                Error = "Email already exists",
                Message = "This email address is already taken."
            };
        }

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

    public async Task<AuthServiceResult> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return new AuthServiceResult { Succeeded = false, Error = "Invalid credentials" };

        // (Valfritt) kräver bekräftad e-post
        // if (!user.EmailConfirmed) return new AuthServiceResult { Succeeded = false, Error = "Email not confirmed" };

        // SignInManager skapar auth-cookie vid lyckad inloggning
        var result = await _signInManager.PasswordSignInAsync(
         user,
         request.Password,
         isPersistent: false,
         lockoutOnFailure: false);

        if (result.Succeeded)
            return new AuthServiceResult { Succeeded = true, Message = "Login successful" };

        if (result.IsLockedOut)
            return new AuthServiceResult { Succeeded = false, Error = "Locked out" };

        if (result.IsNotAllowed)
            return new AuthServiceResult { Succeeded = false, Error = "Not allowed" };
        return new AuthServiceResult { Succeeded = false, Error = "Invalid credentials" };
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

        await _signInManager.RefreshSignInAsync(user);

        return new AuthServiceResult { Succeeded = true, Message = "Password changed successfully." };
    }

    public async Task<AuthServiceResult> ExistsEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is not null)
        {
            return new AuthServiceResult
            {
                Succeeded = false,
                Error = "Email already exists."
            };
        }

        return new AuthServiceResult
        {
            Succeeded = true,
            Message = "Email is available."
        };
    }
}