using Infrastructure.Dtos;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace authsystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly SignInManager<IdentityUser> _signInManager = signInManager;
    private readonly UserManager<IdentityUser> _userManager = userManager;



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

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Unauthorized("Invalid credentials.");

        var result = await _signInManager.PasswordSignInAsync(user, request.Password, isPersistent: false,      
         lockoutOnFailure: false);

        if (!result.Succeeded)
            return Unauthorized("Invalid credentials.");

        // Här skapas en auth-cookie automatiskt
        return Ok("Login successful.");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var result = await _authService.LogoutAsync();

        if (!result.Succeeded)
            return StatusCode(500, result.Error);

        return NoContent();
    }
}
