using Microsoft.AspNetCore.Authorization;

namespace Modules.Common.Infrastructure.Authorization;

/// <summary>
/// A requirement that specifies a permission needed for authorization. It implements the IAuthorizationRequirement interface and contains a single property, Permission, which represents the specific permission required for accessing a resource or performing an action. This requirement can be used in conjunction with authorization policies to enforce access control based on user permissions, allowing for fine-grained authorization decisions throughout the application. By defining specific permissions as requirements, the application can ensure that only users with the appropriate permissions are granted access to certain resources or allowed to perform certain actions, enhancing security and access control within the application.
/// </summary>
internal sealed class PermissionRequirement : IAuthorizationRequirement
{
	public PermissionRequirement(string permission)
	{
		Permission = permission;
	}

	public string Permission { get; }
}
