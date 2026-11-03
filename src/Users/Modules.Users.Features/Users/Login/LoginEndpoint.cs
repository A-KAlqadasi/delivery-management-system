
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
		app.MapPost("/api/users/login", Handle)
		.AllowAnonymous()
		.WithTags(Tags.Users);
	}
	private static async Task<IResult> Handle(
		[FromBody] LoginCommand request,
		ISender sender,
		CancellationToken cancellationToken)
	{
		var result = await sender.Send(request, cancellationToken);

		return result.Match(s => Results.Ok(s), e => e.ToProblem());
	}
}
