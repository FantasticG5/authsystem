using System.Security.Claims;
using Data.Entities;
using Infrastructure.Dtos;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace authsystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;



    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);

        if (!result.Succeeded)
        {
            return BadRequest(result.Error);
        }

        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.LoginAsync(request);


        if (!result.Succeeded)
            return Unauthorized("Invalid credentials.");

        // Här skapas en auth-cookie automatiskt
        return Ok("Login successful.");
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        // plocka userId från claims
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(idClaim))
            return Unauthorized();

        // hämta användaren från Identity
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();

        return Ok(new
        {
            id = user.Id,
            email = user.Email,
            userName = user.UserName,
            // lägg till fler fält om du har, ex FullName
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var result = await _authService.LogoutAsync();

        if (!result.Succeeded)
            return StatusCode(500, result.Error);

        return Ok("Logged out successfully");
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = _userManager.GetUserId(User);

        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _authService.ChangePasswordAsync(userId, request);
        return result.Succeeded ? Ok(result.Message) : BadRequest(new { error = result.Error });
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.UpdateMyProfileAsync(User, request);

        if (!result.Succeeded)
        {
            if (string.Equals(result.Error, "Unauthorized", StringComparison.OrdinalIgnoreCase))
                return Unauthorized();

            return BadRequest(new { error = result.Error });
        }

        return Ok(new
        {
            message = result.Message ?? "Ändringarna är sparade",
            user = new
            {
                id = result.Data!.Id,
                email = result.Data!.Email,
                userName = result.Data!.Email,
                firstname = result.Data!.Firstname,
                lastname = result.Data!.Lastname,
                phoneNumber = result.Data!.PhoneNumber
            }
        });
    }
}
