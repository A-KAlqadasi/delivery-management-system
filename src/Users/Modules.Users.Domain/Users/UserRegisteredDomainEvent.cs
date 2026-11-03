using Modules.Common.Domain;

namespace Modules.Users.Domain.Users;

public sealed class UserRegisteredDomainEvent : DomainEvent
{
	public Guid UserId { get; }
	public UserRegisteredDomainEvent(Guid userId)
	{
		UserId = userId;
	}
}
