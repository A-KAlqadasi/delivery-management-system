using System.Security.Claims;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using Modules.Common.Api.Abstractions;
using Modules.Common.Api.Extensions;
using Modules.Common.Application.Authorization;
using Modules.Common.Application.MediatR;
using Modules.Common.Domain.Results;
using Modules.Users.Domain.Users;
using Modules.Users.Features.Users.Shared.Responses;

namespace Modules.Users.Features.Users.GetUser;

public sealed class GetUserEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapGet("/api/users/profile", async (ClaimsPrincipal claims, ISender sender) =>
		{
			Result<UserResponse> result = await sender.Send(new GetUserQuery(claims.GetUserId()));

			return result.Match(Results.Ok, e => e.ToProblem());
		})
		.RequireAuthorization(Permission.GetUser.Code)
		.WithTags(Tags.Users);
	}
}
