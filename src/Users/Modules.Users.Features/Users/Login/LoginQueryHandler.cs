using Microsoft.Extensions.Logging;

using Modules.Common.Application.MediatR;
using Modules.Common.Domain.Results;
using Modules.Users.Features.Abstractions.Identity;

namespace Modules.Users.Features.Users.Login;

public class LoginQueryHandler(ILogger<LoginQueryHandler> logger, IIdentityProviderService identityService, ITokenProvider tokenProvider)
    : IRequestHandler<LoginRequest, Result<TokenResponse>>
{

    public async Task<Result<TokenResponse>> Handle(LoginRequest query, CancellationToken cancellationToken)
    {
        var userResponse = await identityService.AuthenticateAsync(query.Email, query.Password);

        if (userResponse.IsError)
        {
            return userResponse.Errors;
        }

        var generateTokenResult = await tokenProvider.GenerateJwtTokenAsync(userResponse.Value, cancellationToken);

        if (generateTokenResult.IsError)
        {
            logger.LogError("Generate token error occurred: {ErrorDescription}", generateTokenResult.TopError.Description);

            return generateTokenResult.Errors;
        }

        return generateTokenResult.Value;
    }
}
