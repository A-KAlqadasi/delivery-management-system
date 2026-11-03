using Modules.Common.Domain;
using Modules.Common.Domain.Results;

namespace Modules.Users.Domain.Users;

using Microsoft.AspNetCore.Identity;

public sealed class User : IdentityUser<Guid>
{
	private readonly List<Role> _roles = [];

	private User() { }

	public string FirstName { get; private set; } = default!;
	public string LastName { get; private set; } = default!;

	public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

	public static Result<User> Create(
		string email,
		string firstName,
		string lastName,
		Role role)
	{
		var user = new User
		{
			Id = Guid.NewGuid(),
			UserName = email,
			Email = email,
			NormalizedEmail = email.ToUpperInvariant(),
			NormalizedUserName = email.ToUpperInvariant(),
			FirstName = firstName,
			LastName = lastName,
			EmailConfirmed = true
		};

		user._roles.Add(role);

		return user;
	}

}
