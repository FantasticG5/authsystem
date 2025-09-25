using Data.Entities;
using Infrastructure.Dtos;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class AuthService(UserManager<ApplicationUser> userManager) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

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

        return result.Succeeded
        ? new AuthServiceResult { Succeeded = true }
        : new AuthServiceResult { Succeeded = false, Error = errorMessage, Message = "Failed to create user, debuga för mer info" };

    }

}
