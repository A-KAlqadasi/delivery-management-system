using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Modules.Common.Application.MediatR;
using Modules.Common.Domain.Results;
using Modules.Users.Domain.Users;
using Modules.Users.Features.Abstractions.Data;
using Modules.Users.Features.Abstractions.Identity;

namespace Modules.Users.Features.Users.Login;

public record LoginCommand(
	string Email,
	string Password) : IRequest<Result<TokenResponse>>;


internal sealed class LoginCommandHandler(
	UserManager<User> userManager,
	SignInManager<User> signInManager,
	IUsersDbContext context,
	ITokenProvider tokenProvider)
	: IRequestHandler<LoginCommand, Result<TokenResponse>>
{
	public async Task<Result<TokenResponse>> Handle(
		LoginCommand request,
		CancellationToken cancellationToken)
	{
		User? user = await userManager.FindByEmailAsync(request.Email);

		if (user is null)
		{
			return UserErrors.InvalidCredentials;
		}

		SignInResult signInResult =
			await signInManager.CheckPasswordSignInAsync(
				user,
				request.Password,
				lockoutOnFailure: true);

		if (!signInResult.Succeeded)
		{
			return UserErrors.InvalidCredentials;
		}

		List<string> roles = await context.Users
			.Where(u => u.Id == user.Id)
			.SelectMany(u => u.Roles)
			.Select(r => r.Name)
			.ToListAsync(cancellationToken);

		List<string> permissions = await context.Users
			.Where(u => u.Id == user.Id)
			.SelectMany(u => u.Roles)
			.SelectMany(r => r.Permissions)
			.Select(p => p.Code)
			.Distinct()
			.ToListAsync(cancellationToken);

		UserDetailsDto userDetails = new(
			user.Id,
			user.Email!,
			roles,
			permissions);

		return await tokenProvider.GenerateJwtTokenAsync(
			userDetails,
			cancellationToken);
	}
}
