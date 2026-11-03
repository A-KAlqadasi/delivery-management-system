using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Modules.Common.Domain.Results;
using Modules.Users.Domain.Users;
using Modules.Users.Features.Abstractions.Identity;
using Modules.Users.Infrastructure.Database;

namespace Modules.Users.Infrastructure.Identity;

internal class TokenProvider(IOptions<JwtSettings> configuration, UsersDbContext context) : ITokenProvider
{

	public async Task<Result<TokenResponse>> GenerateJwtTokenAsync(UserDetailsDto user, CancellationToken ct = default)
	{
		var tokenResult = await CreateAsync(user, ct);

		if (tokenResult.IsError)
		{
			return tokenResult.Errors;
		}

		return tokenResult.Value;
	}

	public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
	{
		var tokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(configuration.Value.Secret)),

			ValidateIssuer = true,
			ValidIssuer = configuration.Value.Issuer,

			ValidateAudience = true,
			ValidAudience = configuration.Value.Audience,

			// we want to read even expired tokens
#pragma warning disable CA5404
			ValidateLifetime = false,
#pragma warning restore CA5404

			ClockSkew = TimeSpan.Zero
		};

		var tokenHandler = new JwtSecurityTokenHandler();

		var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

		if (securityToken is not JwtSecurityToken jwtSecurityToken ||
			!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
		{
			throw new SecurityTokenException("Invalid token.");
		}

		return principal;
	}

	private async Task<Result<TokenResponse>> CreateAsync(
	UserDetailsDto user,
	CancellationToken ct = default)
	{
		var expires = DateTime.UtcNow.AddMinutes(
			configuration.Value.TokenExpirationInMinutes);

		var claims = CreateClaims(user);
		var tokenHandler = new JwtSecurityTokenHandler();

		var securityToken = tokenHandler.CreateToken(
			CreateTokenDescriptor(claims, expires));

		var refreshTokenResult = CreateRefreshToken(user.UserId.ToString());

		if (refreshTokenResult.IsError)
		{
			return refreshTokenResult.Errors;
		}

		var refreshToken = refreshTokenResult.Value;

		await SaveRefreshTokenAsync(user.UserId.ToString(), refreshToken, ct);

		return new TokenResponse
		{
			AccessToken = tokenHandler.WriteToken(securityToken),
			RefreshToken = refreshToken.Token,
			ExpiresOnUtc = expires
		};
	}

	private static List<Claim> CreateClaims(UserDetailsDto user)
	{
		var claims = new List<Claim>
	{
		new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
		new(JwtRegisteredClaimNames.UniqueName, user.Email),
		new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
		new(ClaimTypes.Role, user.Roles.First())
	};

		claims.AddRange(user.Permissions.Select(
			permission => new Claim("permission", permission)));

		return claims;
	}

	private SecurityTokenDescriptor CreateTokenDescriptor(
		List<Claim> claims,
		DateTime expires)
	{
		var settings = configuration.Value;

		return new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = expires,
			Issuer = settings.Issuer,
			Audience = settings.Audience,
			SigningCredentials = new SigningCredentials(
				new SymmetricSecurityKey(
					Encoding.UTF8.GetBytes(settings.Secret)),
				SecurityAlgorithms.HmacSha256Signature)
		};
	}

	private static Result<RefreshToken> CreateRefreshToken(string userId)
	{
		return RefreshToken.Create(
			Guid.NewGuid(),
			GenerateRefreshToken(),
			userId,
			DateTimeOffset.UtcNow.AddDays(7));
	}

	private async Task SaveRefreshTokenAsync(
		string userId,
		RefreshToken refreshToken,
		CancellationToken ct)
	{
		var strategy = context.Database.CreateExecutionStrategy();

		await strategy.ExecuteAsync(async () =>
		{
			await using var transaction =
				await context.Database.BeginTransactionAsync(ct);

			await context.RefreshTokens
				.Where(rt => rt.UserId == userId)
				.ExecuteDeleteAsync(ct);

			context.RefreshTokens.Add(refreshToken);

			await context.SaveChangesAsync(ct);
			await transaction.CommitAsync(ct);
		});
	}

	private static string GenerateRefreshToken()
	{
		return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
	}
}

