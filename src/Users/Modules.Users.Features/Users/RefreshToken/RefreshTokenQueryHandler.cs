using System.Security.Claims;

using Microsoft.Extensions.Logging;

using Modules.Common.Application.MediatR;
using Modules.Common.Domain.Results;
using Modules.Users.Domain.Users;
using Modules.Users.Features.Abstractions.Identity;
using Modules.Users.Features.Users.GetUser;
using Modules.Users.Features.Users.GetUserPermissions;

namespace Modules.Users.Features.Users.RefreshToken;

public class RefreshTokenQueryHandler(ILogger<RefreshTokenQueryHandler> logger, IIdentityProviderService identityService, ITokenProvider tokenProvider, ISender sender)
    : IRequestHandler<RefreshTokenQuery, Result<TokenResponse>>
{

    public async Task<Result<TokenResponse>> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
    {
        var principal = tokenProvider.GetPrincipalFromExpiredToken(request.ExpiredAccessToken);

        if (principal is null)
        {
            logger.LogError("Expired access token is not valid");

            return UserErrors.ExpiredAccessTokenInvalid;
        }

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
        {
            logger.LogError("Invalid userId claim");

            return UserErrors.UserIdRequired;
        }

        var userIdGuid = Guid.Parse(userId);

        var userResult = await sender.Send(new GetUserQuery(userIdGuid), cancellationToken);

        if (userResult.IsError)
        {
            logger.LogError("Get user by id error occurred: {ErrorDescription}", userResult.TopError.Description);
            return userResult.Errors;
        }

        var userPermissions = await sender.Send(new GetUserPermissionsQuery(userId), cancellationToken);

        var refreshToken = await identityService.GetRefreshTokenAsync(request.RefreshToken, userId);

        if (refreshToken is null || refreshToken.Value.ExpiresOnUtc < DateTime.UtcNow)
        {
            logger.LogError("Refresh token has expired");

            return UserErrors.RefreshTokenIdRequired;
        }

        var user = new UserDetailsDto
        {
            UserId = userResult.Value.Id.ToString(),
            Email = userResult.Value.Email!,
            Permissions = userPermissions.Value.Permissions.ToList()
        };


        var generateTokenResult = await tokenProvider.GenerateJwtTokenAsync(user, cancellationToken);

        if (generateTokenResult.IsError)
        {
            logger.LogError("Generate token error occurred: {ErrorDescription}", generateTokenResult.TopError.Description);

            return generateTokenResult.Errors;
        }

        return generateTokenResult.Value;
    }
}
