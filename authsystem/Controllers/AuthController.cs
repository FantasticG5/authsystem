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
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var reg = await _authService.RegisterAsync(request);
        if (!reg.Succeeded)
            return BadRequest(new { error = reg.Error });

        // Autologga in med samma credsen och returnera tokens
        var loginRes = await _authService.LoginAsync(new LoginRequest
        {
            Email = request.Email,
            Password = request.Password
        });

        if (!loginRes.Succeeded)
            return StatusCode(500, new { error = "Registration succeeded but auto-login failed." });

        // Viktigt: returnera tokenpayloaden så klienten kan spara tokens
        return Ok(loginRes);           // eller return Ok(loginRes.Data);
    }

    [HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequest request)
{
    if (!ModelState.IsValid) return BadRequest(ModelState);

    var result = await _authService.LoginAsync(request);
    if (!result.Succeeded)
        return Unauthorized(new { error = "Invalid credentials" });

    return Ok(result); // eller Ok(result.Data)
}

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[HttpGet("me")]
public async Task<IActionResult> Me()
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
              ?? User.FindFirst("sub")?.Value;
    if (string.IsNullOrEmpty(userId)) return Unauthorized();

    var user = await _userManager.FindByIdAsync(userId);
    if (user is null) return Unauthorized();

    return Ok(new { id = user.Id, email = user.Email, userName = user.UserName });
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
}
