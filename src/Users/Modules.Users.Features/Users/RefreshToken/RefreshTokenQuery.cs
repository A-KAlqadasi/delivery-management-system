using Modules.Common.Application.MediatR;
using Modules.Common.Domain.Results;
using Modules.Users.Features.Abstractions.Identity;

namespace Modules.Users.Features.Users.RefreshToken;

public record RefreshTokenQuery(string RefreshToken, string ExpiredAccessToken) : IRequest<Result<TokenResponse>>;
