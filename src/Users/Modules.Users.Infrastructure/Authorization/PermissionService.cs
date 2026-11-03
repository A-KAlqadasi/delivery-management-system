using Modules.Common.Application.Authorization;
using Modules.Common.Application.MediatR;
using Modules.Common.Domain.Results;
using Modules.Users.Features.Users.GetUserPermissions;

namespace Modules.Users.Infrastructure.Authorization;

internal sealed class PermissionService(ISender sender) : IPermissionService
{
	public async Task<Result<PermissionsResponse>> GetUserPermissionsAsync(string identityId)
	{
		return await sender.Send(new GetUserPermissionsQuery(identityId));
	}
}

