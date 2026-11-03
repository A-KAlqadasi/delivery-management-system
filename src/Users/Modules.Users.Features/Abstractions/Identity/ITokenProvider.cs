using System.Security.Claims;

using Modules.Common.Domain.Results;

namespace Modules.Users.Features.Abstractions.Identity;

public interface ITokenProvider
{
    Task<Result<TokenResponse>> GenerateJwtTokenAsync(UserDetailsDto user, CancellationToken ct = default);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
