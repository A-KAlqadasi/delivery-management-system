using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using Modules.Common.Api.Abstractions;
using Modules.Common.Api.Extensions;
using Modules.Common.Application.MediatR;
using Modules.Common.Domain.Results;
using Modules.Users.Domain.Users;

namespace Modules.Users.Features.Users.RegisterUser;

public sealed class RegisterUserEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapPost("/api/users/register", async (RegisterUserCommand request, ISender sender) =>
		{
			Result<string> result = await sender.Send(request);

			return result.Match(Results.Ok, errors => errors.ToProblem());
		})
		.RequireAuthorization(Permission.CreateUser.Code)
		.WithTags(Tags.Users);
	}
}
