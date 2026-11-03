using Modules.Common.Domain;

namespace Modules.Users.Domain.Users;

public sealed class User : Entity
{
	private readonly List<Role> _roles = [];
	private User()
	{
	}

	public Guid Id { get; private set; }

	public string Email { get; private set; } = default!;

	public string FirstName { get; private set; } = default!;

	public string LastName { get; private set; } = default!;
	public string IdentityId { get; private set; } = default!;

	public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

	public static User Create(string email, string firstName, string lastName, string identityId)
	{
		User user = new User
		{
			Id = Guid.NewGuid(),
			Email = email,
			FirstName = firstName,
			LastName = lastName,
			IdentityId = identityId,
		};

		user._roles.Add(Role.Admin);
		user.Raise(new UserRegisteredDomainEvent(user.Id));

		return user;
	}

	public void Update(string firstName, string lastName)
	{
		if (FirstName == firstName && LastName == lastName)
		{
			return;
		}

		FirstName = firstName;
		LastName = lastName;

		Raise(new UserProfileUpdatedDomainEvent(Id, FirstName, LastName));
	}
}
