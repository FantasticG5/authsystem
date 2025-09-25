using Infrastructure.Dtos;
using Infrastructure.Models;

namespace Infrastructure.Interfaces;

public interface IAuthService
{
    Task<AuthServiceResult> RegisterAsync(RegisterRequest request);
}