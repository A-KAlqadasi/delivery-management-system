using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Modules.Common.Application.MediatR;
using Modules.Common.Domain.Results;
using Modules.Users.Domain.Users;
using Modules.Users.Features.Abstractions.Data;
using Modules.Users.Features.Abstractions.Identity;

namespace Modules.Users.Features.Users.RegisterUser;


internal sealed record RegisterUserCommand(
	string Email,
	string Password,
	string FirstName,
	string LastName,
	string Role) : IRequest<Result<string>>;

internal sealed class RegisterUserCommandHandler(
	IUsersDbContext context,
	UserManager<User> userManager)
	: IRequestHandler<RegisterUserCommand, Result<string>>
{
	public async Task<Result<string>> Handle(
		RegisterUserCommand request,
		CancellationToken cancellationToken)
	{
		var role = await context.Roles
			.FirstOrDefaultAsync(
				r => r.Name == request.Role,
				cancellationToken);

		if (role is null)
		{
			return UserErrors.RoleNotFound;
		}

		var user = User.Create(
			request.Email,
			request.FirstName,
			request.LastName,
			role);

		var result = await userManager.CreateAsync(
			user.Value,
			request.Password);

		if (!result.Succeeded)
		{
			if (result.Errors.Any(e =>
				e.Code is "DuplicateEmail" or "DuplicateUserName"))
			{
				return IdentityProviderErrors.EmailIsNotUnique;
			}

			return IdentityProviderErrors.RegistrationFailed;
		}

		return user.Value.Id.ToString();
	}
}
