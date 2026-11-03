
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Modules.Common.Api.Abstractions;
using Modules.Common.Api.Extensions;
using Modules.Common.Application.MediatR;
using Modules.Common.Domain.Results;
using Modules.Users.Features.Abstractions.Identity;

namespace Modules.Users.Features.Users.Login;

public sealed class LoginEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapPost("users/login", async (
			LoginRequest request,
			ISender sender) =>
		{
			Result<TokenResponse> result = await sender.Send(request);

			return result.Match(Results.Ok, e => e.ToProblem());
		})
		.RequireAuthorization("users:create")
		.WithTags(Tags.Users);
	}
}
