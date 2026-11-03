using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Modules.Common.Application.MediatR;
using Modules.Common.Domain.Results;
using Modules.Users.Domain.Users;
using Modules.Users.Features.Abstractions.Identity;
using Modules.Users.Features.Users.GetUserPermissions;
using Modules.Users.Infrastructure.Database;

namespace Modules.Users.Infrastructure.Identity;

internal sealed class IdentityProviderService(
	UserManager<ApplicationIdentityUser> userManager,
	UsersDbContext dbContext,
	ILogger<IdentityProviderService> logger, ISender sender)
	: IIdentityProviderService
{
	public async Task<Result<string>> RegisterUserAsync(
		UserModel user,
		CancellationToken cancellationToken = default)
	{
		ApplicationIdentityUser identityUser = new()
		{
			UserName = user.Email,
			Email = user.Email,
			NormalizedEmail = user.Email.ToUpper(),
			EmailConfirmed = true
		};

		IdentityResult result = await userManager.CreateAsync(
			identityUser,
			user.Password);

		if (!result.Succeeded)
		{
			foreach (IdentityError error in result.Errors)
			{
				logger.LogWarning(
					"Identity registration failed: {Code} - {Description}",
					error.Code,
					error.Description);
			}

			if (result.Errors.Any(e =>
				e.Code is "DuplicateEmail" or "DuplicateUserName"))
			{
				return IdentityProviderErrors.EmailIsNotUnique;
			}

			return IdentityProviderErrors.RegistrationFailed;
		}

		return identityUser.Id;
	}


	public async Task<Result<UserDetailsDto>> AuthenticateAsync(string userName, string password)
	{
		var user = await userManager.FindByEmailAsync(userName);

		if (user is null)
		{
			return UserErrors.InvalidCredentials;
		}

		if (!await userManager.CheckPasswordAsync(user, password))
		{
			return UserErrors.InvalidCredentials;
		}


		var permissions = await sender.Send(new GetUserPermissionsQuery(user.Id), CancellationToken.None);

		var dto = new UserDetailsDto
		{
			UserId = user.Id,
			Email = user.Email!,
			Permissions = permissions.Value.Permissions.ToList()
		};

		return dto;
	}

	public async Task<Result<RefreshToken>> GetRefreshTokenAsync(string refreshToken, string userId)
	{
		var token = await dbContext.RefreshTokens
			.AsNoTracking()
			.FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);

		if (token is null)
		{
			return Error.NotFound(
				"RefreshToken_Not_Found",
				"Refresh token is invalid.");
		}

		return token;
	}

}
