
using Modules.Common.Domain.Results;

namespace Modules.Users.Domain.Users;

public static class UserErrors
{
	public static Error NotFound(Guid userId) =>
		Error.NotFound("Users.NotFound", $"The user with the identifier {userId} not found");

	public static Error NotFound(string identityId) =>
		Error.NotFound("Users.NotFound", $"The user with the IDP identifier {identityId} not found");

	public static Error InvalidCredentials => Error.Conflict("Users.InvalidCredentials", "Invalid username or password");

	public static readonly Error RefreshTokenIdRequired =
	   Error.Validation("RefreshToken.IdRequired", "Refresh token ID is required.");

	public static readonly Error TokenRequired =
		Error.Validation("RefreshToken.TokenRequired", "Token value is required.");

	public static readonly Error UserIdRequired =
		Error.Validation("RefreshToken.UserIdRequired", "User ID is required.");

	public static readonly Error ExpiryInvalid =
		Error.Validation("RefreshToken.ExpiryInvalid", "Expiry date must be in the future.");

	public static readonly Error ExpiredAccessTokenInvalid =
		Error.Validation("RefreshToken.ExpiredAccessTokenInvalid", "Expired access token is not valid.");

	public static readonly Error RoleNotFound = Error.NotFound("Roles.NotFound", "Role is not found");

}
