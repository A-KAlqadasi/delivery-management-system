using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

using Modules.Common.Application.Authorization;
using Modules.Common.Application.Exceptions;
using Modules.Common.Domain.Results;

namespace Modules.Common.Infrastructure.Authorization;

/// <summary>
/// A custom implementation of IClaimsTransformation that enriches the ClaimsPrincipal with additional claims based on the user's permissions. It retrieves the user's identity ID from the existing claims, uses a permission service to fetch the user's permissions, and then adds new claims for the user ID and permissions to the ClaimsPrincipal. This transformation allows for dynamic enrichment of the user's claims based on their permissions, enabling more flexible and fine-grained authorization throughout the application. If any issues arise during the retrieval of permissions or if required claims are missing, appropriate exceptions are thrown to indicate the problem with transforming the claims. This class is typically registered as a service in the application's dependency injection container to be used during the authentication process to transform the user's claims before they are used for authorization decisions.
/// </summary>
/// <param name="serviceScopeFactory"></param>
internal sealed class CustomClaimsTransformation(IServiceScopeFactory serviceScopeFactory) : IClaimsTransformation
{
	public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
	{
		if (principal.HasClaim(c => c.Type == CustomClaims.Sub))
		{
			return principal;
		}

		using IServiceScope scope = serviceScopeFactory.CreateScope();

		IPermissionService permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();

		string identityId = principal.GetIdentityId();

		Result<PermissionsResponse> result = await permissionService.GetUserPermissionsAsync(identityId);

		if (result.IsError)
		{
			throw new MyModularException(nameof(IPermissionService.GetUserPermissionsAsync), result.TopError);
		}

		ClaimsIdentity claimsIdentity = new ClaimsIdentity();

		claimsIdentity.AddClaim(new Claim(CustomClaims.Sub, result.Value.UserId.ToString()));

		foreach (string permission in result.Value.Permissions)
		{
			claimsIdentity.AddClaim(new Claim(CustomClaims.Permission, permission));
		}

		principal.AddIdentity(claimsIdentity);

		return principal;
	}
}
