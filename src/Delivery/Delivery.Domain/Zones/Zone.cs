using Modules.Common.Domain;
using Modules.Common.Domain.Results;

namespace Delivery.Domain.Zones;


public class Zone : Entity
{
	public Guid Id { get; private set; }
	public string Name { get; private set; } = default!;
	public decimal? DeliveryFee { get; private set; }

	public Guid? ParentZoneId { get; private set; }
	public Zone? ParentZone { get; private set; }

	// Optional: children navigation for convenience
	private readonly List<Zone> _children = new();
	public IReadOnlyCollection<Zone> Children => _children.AsReadOnly();

	private Zone() { } // EF Core

	private Zone(Guid id, string name, decimal? deliveryFee, Guid? parentZoneId)
	{
		Id = id;
		Name = name;
		DeliveryFee = deliveryFee;
		ParentZoneId = parentZoneId;
	}

	public static Result<Zone> Create(string name, decimal? deliveryFee, Guid? parentZoneId = null)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			return DeliveryErrors.ZoneNameRequired;
		}


		var zone = new Zone(Guid.NewGuid(), name, deliveryFee, parentZoneId);
		return zone;
	}

	/// <summary>
	/// Assigns (or clears) the parent zone. Pass null to make this a root zone.
	/// </summary>
	public Result<Success> AssignParent(Zone? parent)
	{
		if (parent is null)
		{
			ParentZoneId = null;
			ParentZone = null;
			return Result.Success;
		}

		if (parent.Id == Id)
		{
			return DeliveryErrors.ZoneCannotBeItsOwnParent;
		}

		// Prevent cycles: a zone cannot be assigned a parent that is its descendant.
		if (IsDescendantOf(parent))
		{
			return DeliveryErrors.ZoneHierarchyCycle;
		}

		ParentZoneId = parent.Id;
		ParentZone = parent;
		return Result.Success;
	}

	public Result<Success> Rename(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			return DeliveryErrors.ZoneNameRequired;
		}
		Name = name;
		return Result.Success;
	}

	public Result<Success> ChangeDeliveryFee(decimal? fee)
	{
		if (fee is < 0)
		{
			return DeliveryErrors.DeliveryFeeCannotBeNegative;
		}
		DeliveryFee = fee;
		return Result.Success;
	}

	/// <summary>
	/// Returns true if <paramref name="candidate"/> is an ancestor of this zone
	/// (i.e. assigning it as parent would create a cycle).
	/// </summary>
	private bool IsDescendantOf(Zone candidate)
	{
		// Walk up from candidate's ancestors; if we hit `this`, cycle.
		var current = candidate;
		var guard = 0;
		while (current is not null && guard++ < 1000) // safety guard
		{
			if (current.Id == Id)
			{
				return true;
			}
			current = current.ParentZone;
		}
		return false;
	}
}
