using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using Modules.Common.Api.Abstractions;
using Modules.Common.Api.Extensions;
using Modules.Common.Application.MediatR;
using Modules.Common.Domain.Results;
using Modules.Users.Domain.Users;
using Modules.Users.Features.Abstractions.Data;
using Modules.Users.Features.Abstractions.Identity;

namespace Modules.Users.Features.Users.RegisterUser;

public sealed record RegisterUserRequest(string Email, string Password, string FirstName, string LastName);


internal sealed record RegisterUserCommand(string Email, string Password, string FirstName, string LastName) : IRequest<Result<Guid>>;


internal sealed class RegisterUserCommandHandler(IUsersDbContext context, IIdentityProviderService identityProviderService)
	: IRequestHandler<RegisterUserCommand, Result<Guid>>
{
	public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
	{
		Result<string> result = await identityProviderService.RegisterUserAsync(new UserModel(request.Email, request.Password, request.FirstName, request.LastName), cancellationToken);

		if (result.IsError)
		{
			return result.TopError;
		}

		User user = User.Create(request.Email, request.FirstName, request.LastName, result.Value);

		context.Users.Add(user);

		await context.SaveChangesAsync(cancellationToken);

		return user.Id;
	}
}

public sealed class RegisterUserEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapPost("users/register", async (RegisterUserRequest request, ISender sender) =>
		{
			Result<Guid> result = await sender.Send(new RegisterUserCommand(
				request.Email,
				request.Password,
				request.FirstName,
				request.LastName));

			return result.Match(Results.Ok, errors => errors.ToProblem());
		})
		.AllowAnonymous()
		.WithTags(Tags.Users);
	}
}
