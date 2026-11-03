using Modules.Common.Domain;
using Modules.Common.Domain.Results;

namespace Modules.Users.Domain.Users;

public sealed class RefreshToken : Entity
{
	public Guid Id { get; private set; }
	public string? Token { get; }
	public string? UserId { get; }
	public DateTimeOffset ExpiresOnUtc { get; }

	private RefreshToken()
	{ }

	private RefreshToken(Guid id, string? token, string? userId, DateTimeOffset expiresOnUtc)

	{
		Id = id;
		Token = token;
		UserId = userId;
		ExpiresOnUtc = expiresOnUtc;
	}

	public static Result<RefreshToken> Create(Guid id, string? token, string? userId, DateTimeOffset expiresOnUtc)
	{
		if (id == Guid.Empty)
		{
			return UserErrors.RefreshTokenIdRequired;
		}

		if (string.IsNullOrWhiteSpace(token))
		{
			return UserErrors.TokenRequired;
		}

		if (string.IsNullOrWhiteSpace(userId))
		{
			return UserErrors.UserIdRequired;
		}

		if (expiresOnUtc <= DateTimeOffset.UtcNow)
		{
			return UserErrors.ExpiryInvalid;
		}

		return new RefreshToken(id, token, userId, expiresOnUtc);
	}
}
