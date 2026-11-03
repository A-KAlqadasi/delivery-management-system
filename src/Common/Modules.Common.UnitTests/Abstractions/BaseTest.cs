using Bogus;

using Modules.Common.Domain;

namespace Modules.Common.UnitTests.Abstractions;

public abstract class BaseTest
{

	protected static readonly Faker Faker = new();

	public static T AssertDomainEventWasPublished<T>(Entity entity) where T : IDomainEvent
	{
		T? domainEvent = entity.DomainEvents.OfType<T>().SingleOrDefault();
		if (domainEvent is null)
		{
			throw new Exception($"Expected domain event of type {typeof(T).Name} was not published.");
		}
		return domainEvent;
	}

}
