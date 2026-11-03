using System.Security.Claims;

using Modules.Common.Application.Exceptions;

namespace Modules.Common.Application.Authorization;

/// <summary>
/// Extension methods for ClaimsPrincipal to retrieve user information such as user ID, identity ID, and permissions from the claims. These methods provide a convenient way to access relevant user information stored in the claims for authorization purposes throughout the application. They handle potential cases where the required claims may be missing and throw appropriate exceptions to indicate issues with retrieving user information from the claims.
/// </summary>
public static class ClaimsPrincipalExtensions
{
	public static Guid GetUserId(this ClaimsPrincipal? principal)
	{
		string? userId = principal?.FindFirst(CustomClaims.Sub)?.Value;

		return Guid.TryParse(userId, out Guid parsedUserId) ?
			parsedUserId :
			throw new MyModularException("User identifier is unavailable");
	}

	public static string GetIdentityId(this ClaimsPrincipal? principal)
	{
		return principal?.FindFirst(CustomClaims.IdentityId)?.Value ??
				 throw new MyModularException("User identity is unavailable");
	}

	public static HashSet<string> GetPermissions(this ClaimsPrincipal? principal)
	{
		IEnumerable<Claim> permissionClaims = principal?.FindAll(CustomClaims.Permission) ??
												throw new MyModularException("Permissions are unavailable");

		return [.. permissionClaims.Select(c => c.Value)];
	}
}
