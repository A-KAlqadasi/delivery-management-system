using Modules.Common.Domain.Results;

namespace Modules.Common.Application.Authorization;

public interface IPermissionService
{
	Task<Result<PermissionsResponse>> GetUserPermissionsAsync(string identityId);
}
