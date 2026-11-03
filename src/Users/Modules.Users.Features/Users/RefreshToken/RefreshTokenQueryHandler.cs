using System.Security.Claims;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Modules.Common.Application.Authorization;
using Modules.Common.Application.MediatR;
using Modules.Common.Domain.Results;
using Modules.Users.Domain.Users;
using Modules.Users.Features.Abstractions.Data;
using Modules.Users.Features.Abstractions.Identity;
using Modules.Users.Features.Users.GetUser;

namespace Modules.Users.Features.Users.RefreshToken;

public record RefreshTokenQuery(string RefreshToken, string ExpiredAccessToken) : IRequest<Result<TokenResponse>>;



internal sealed class RefreshTokenQueryHandler(
	ILogger<RefreshTokenQueryHandler> logger,
	IUsersDbContext context,
	ITokenProvider tokenProvider,
	ISender sender)
	: IRequestHandler<RefreshTokenQuery, Result<TokenResponse>>
{
	public async Task<Result<TokenResponse>> Handle(
		RefreshTokenQuery request,
		CancellationToken cancellationToken)
	{
		var userIdResult = GetUserIdFromExpiredToken(
			request.ExpiredAccessToken);

		if (userIdResult.IsError)
		{
			return userIdResult.Errors;
		}

		Guid userId = userIdResult.Value;

		var userResult = await sender.Send(
			new GetUserQuery(userId),
			cancellationToken);

		if (userResult.IsError)
		{
			return userResult.Errors;
		}

		var refreshTokenResult = await ValidateRefreshTokenAsync(
			request.RefreshToken,
			userId,
			cancellationToken);

		if (refreshTokenResult.IsError)
		{
			return refreshTokenResult.Errors;
		}

		var roles = await GetUserRolesAsync(userId, cancellationToken);

		var permissions = await GetUserPermissionsAsync(
			userId,
			cancellationToken);

		var user = new UserDetailsDto(
			UserId: userResult.Value.Id,
			Email: userResult.Value.Email!,
			Roles: roles,
			Permissions: permissions);

		return await tokenProvider.GenerateJwtTokenAsync(
			user,
			cancellationToken);
	}

	private Result<Guid> GetUserIdFromExpiredToken(string expiredAccessToken)
	{
		var principal = tokenProvider.GetPrincipalFromExpiredToken(
			expiredAccessToken);

		if (principal is null)
		{
			logger.LogWarning("Expired access token is invalid.");
			return UserErrors.ExpiredAccessTokenInvalid;
		}

		string? userIdClaim =
			principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
			?? principal.FindFirst(CustomClaims.Sub)?.Value;

		if (!Guid.TryParse(userIdClaim, out Guid userId))
		{
			logger.LogWarning("Invalid user ID claim.");
			return UserErrors.UserIdRequired;
		}

		return userId;
	}

	private async Task<Result<bool>> ValidateRefreshTokenAsync(
		string refreshToken,
		Guid userId,
		CancellationToken cancellationToken)
	{
		var token = await context.RefreshTokens
			.AsNoTracking()
			.FirstOrDefaultAsync(
				rt => rt.Token == refreshToken &&
					  rt.UserId == userId.ToString(),
				cancellationToken);

		if (token is null ||
			token.ExpiresOnUtc <= DateTimeOffset.UtcNow)
		{
			logger.LogWarning(
				"Refresh token is invalid or expired for user {UserId}.",
				userId);

			return UserErrors.RefreshTokenIdRequired;
		}

		return true;
	}

	private async Task<List<string>> GetUserRolesAsync(
		Guid userId,
		CancellationToken cancellationToken)
	{
		return await context.Users
			.Where(u => u.Id == userId)
			.SelectMany(u => u.Roles)
			.Select(r => r.Name)
			.ToListAsync(cancellationToken);
	}

	private async Task<List<string>> GetUserPermissionsAsync(
		Guid userId,
		CancellationToken cancellationToken)
	{
		return await context.Users
			.Where(u => u.Id == userId)
			.SelectMany(u => u.Roles)
			.SelectMany(r => r.Permissions)
			.Select(p => p.Code)
			.Distinct()
			.ToListAsync(cancellationToken);
	}
}
