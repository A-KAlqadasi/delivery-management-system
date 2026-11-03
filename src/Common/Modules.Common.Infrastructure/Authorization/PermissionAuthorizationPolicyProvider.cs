using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Modules.Common.Infrastructure.Authorization;

/// <summary>
/// A custom implementation of IAuthorizationPolicyProvider that dynamically creates authorization policies based on permission requirements. It extends the DefaultAuthorizationPolicyProvider and overrides the GetPolicyAsync method to check if a policy with the given name already exists. If it does, it returns the existing policy; otherwise, it creates a new authorization policy that includes a PermissionRequirement with the policy name as the required permission. The new policy is then added to the AuthorizationOptions for future reference. This allows for dynamic creation of authorization policies based on permissions without needing to predefine all possible policies in the application, enabling more flexible and scalable authorization management.
/// </summary>
internal sealed class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
	private readonly AuthorizationOptions _authorizationOptions;

	public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
		: base(options)
	{
		_authorizationOptions = options.Value;
	}

	public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
	{
		AuthorizationPolicy? policy = await base.GetPolicyAsync(policyName);

		if (policy is not null)
		{
			return policy;
		}

		AuthorizationPolicy permissionPolicy = new AuthorizationPolicyBuilder()
			.AddRequirements(new PermissionRequirement(policyName))
			.Build();

		_authorizationOptions.AddPolicy(policyName, permissionPolicy);

		return permissionPolicy;
	}
}
