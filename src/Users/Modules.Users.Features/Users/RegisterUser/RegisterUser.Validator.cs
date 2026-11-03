using FluentValidation;

namespace Modules.Users.Features.Users.RegisterUser;

internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
	public RegisterUserCommandValidator()
	{
		RuleFor(c => c.FirstName).NotEmpty();
		RuleFor(c => c.LastName).NotEmpty();
		RuleFor(c => c.Email).EmailAddress();
		RuleFor(c => c.Password).MinimumLength(6);
	}
}
