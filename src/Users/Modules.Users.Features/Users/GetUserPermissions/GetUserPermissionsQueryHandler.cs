using Microsoft.EntityFrameworkCore;

using Modules.Common.Application.Authorization;

using Modules.Common.Application.MediatR;
using Modules.Common.Domain.Results;
using Modules.Users.Domain.Users;
using Modules.Users.Features.Abstractions.Data;

namespace Modules.Users.Features.Users.GetUserPermissions;

internal sealed class GetUserPermissionsQueryHandler(IUsersDbContext context)
	: IRequestHandler<GetUserPermissionsQuery, Result<PermissionsResponse>>
{
	public async Task<Result<PermissionsResponse>> Handle(
		GetUserPermissionsQuery request,
		CancellationToken cancellationToken)
	{
		var permissions = await context.Users
		.Where(u => u.IdentityId == request.IdentityId)
		.SelectMany(u => u.Roles,
			(u, role) => new { u.Id, role })
		.SelectMany(x => x.role.Permissions,
			(x, permission) => new UserPermission
			{
				UserId = x.Id,
				Permission = permission.Code
			})
		.Distinct()
		.ToListAsync(cancellationToken);

		if (permissions.Count == 0)
		{
			return UserErrors.NotFound(request.IdentityId);
		}

		return new PermissionsResponse(permissions[0].UserId, [.. permissions.Select(p => p.Permission)]);
	}

}

public sealed class UserPermission
{
	public Guid UserId { get; init; }

	public string Permission { get; init; } = string.Empty;
}
