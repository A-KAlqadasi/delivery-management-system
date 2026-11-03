using Modules.Common.Application.Authorization;
using Modules.Common.Application.MediatR;
using Modules.Common.Domain.Results;

namespace Modules.Users.Features.Users.GetUserPermissions;

public sealed record GetUserPermissionsQuery(string IdentityId) : IRequest<Result<PermissionsResponse>>;
