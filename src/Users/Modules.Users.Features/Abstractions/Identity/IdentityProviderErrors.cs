using Modules.Common.Domain.Results;

namespace Modules.Users.Features.Abstractions.Identity;

public static class IdentityProviderErrors
{
	public static readonly Error EmailIsNotUnique = Error.Conflict(
		"Identity.EmailIsNotUnique",
		"The specified email is not unique.");

	public static readonly Error RegistrationFailed =
	Error.Conflict(
		"Identity.RegistrationFailed",
		"User registration failed.");
}
