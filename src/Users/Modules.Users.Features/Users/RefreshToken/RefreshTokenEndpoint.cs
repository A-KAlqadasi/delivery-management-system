using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Modules.Common.Api.Abstractions;
using Modules.Common.Api.Extensions;
using Modules.Common.Application.MediatR;
using Modules.Common.Domain.Results;
using Modules.Users.Features.Abstractions.Identity;

namespace Modules.Users.Features.Users.RefreshToken;

public sealed class RefreshTokenEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapPost("/api/users/refresh-token", async (
			RefreshTokenQuery request,
			ISender sender,
			CancellationToken ct) =>
		{
			Result<TokenResponse> result = await sender.Send(request, ct);

			return result.Match(Results.Ok, e => e.ToProblem());
		})
		.AllowAnonymous()
		.WithTags(Tags.Users);
	}
}
