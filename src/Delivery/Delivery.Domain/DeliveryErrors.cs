using Modules.Common.Domain.Results;

namespace Delivery.Domain;


public static class DeliveryErrors
{
	public static Error ZoneNotFound =>
		Error.NotFound("Zones.ZoneNotFound", "The specified zone does not exist");
	public static Error ZoneNameRequired =>
		Error.Validation("Zones.NameRequired", "The zone name is required");

	public static Error ZoneCannotBeItsOwnParent =>
		Error.Conflict("Zones.CannotBeItsOwnParent", "A zone cannot be its own parent");

	public static Error ZoneHierarchyCycle =>
		Error.Conflict("Zones.HierarchyCycle", "A zone cannot be assigned a parent that is its descendant");

	public static Error DeliveryFeeCannotBeNegative =>
		Error.Validation("Zones.DeliveryFeeCannotBeNegative", "Delivery fee cannot be negative");

	public static Error ParentZoneNotFound =>
		Error.NotFound("Zones.ParentZoneNotFound", "The specified parent zone does not exist");
}
