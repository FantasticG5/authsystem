using Data.Entities;

namespace Infrastructure.Interfaces;

public interface IJwtTokenService
{
    (string token, DateTime expiresUtc) CreateAccessToken(ApplicationUser user, IEnumerable<string> roles, IDictionary<string,string>? extraClaims = null);
    (string token, DateTime expiresUtc) CreateRefreshToken(ApplicationUser user);
}