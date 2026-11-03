using Microsoft.AspNetCore.Authorization;

using Modules.Common.Application.Authorization;

namespace Modules.Common.Infrastructure.Authorization;

/// <summary>
/// An authorization handler that checks if the user has the required permission to access a resource. It implements the AuthorizationHandler<PermissionRequirement> class and overrides the HandleRequirementAsync method to check if the user's claims contain the required permission specified in the PermissionRequirement. If the user has the required permission, the requirement is marked as succeeded, allowing access to the resource. This handler is typically registered in the application's dependency injection container and used in conjunction with authorization policies that require specific permissions for accessing certain resources or performing certain actions within the application.
/// </summary>
internal sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
	protected override Task HandleRequirementAsync(
		AuthorizationHandlerContext context,
		PermissionRequirement requirement)
	{
		HashSet<string> permissions = context.User.GetPermissions();

		if (permissions.Contains(requirement.Permission))
		{
			context.Succeed(requirement);
		}

		return Task.CompletedTask;
	}
}
