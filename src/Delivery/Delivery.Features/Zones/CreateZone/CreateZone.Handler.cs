using Delivery.Domain;
using Delivery.Domain.Zones;
using Delivery.Features.Abstractions;

using Microsoft.EntityFrameworkCore;

using Modules.Common.Application.MediatR;
using Modules.Common.Domain.Results;

namespace Delivery.Features.Zones.CreateZone;

internal sealed record CreateZoneCommand(
	string Name,
	decimal? DeliveryFee,
	Guid? ParentZoneId) : IRequest<Result<Guid>>;

internal sealed class CreateZoneCommandHandler(IDeliveryDbContext context)
	: IRequestHandler<CreateZoneCommand, Result<Guid>>
{
	public async Task<Result<Guid>> Handle(
		CreateZoneCommand request,
		CancellationToken cancellationToken)
	{
		if (request.ParentZoneId is Guid parentId)
		{
			var parentExists = await context.Zones
				.AnyAsync(z => z.Id == parentId, cancellationToken);

			if (!parentExists)
			{
				return DeliveryErrors.ParentZoneNotFound;
			}
		}

		var zone = Zone.Create(request.Name, request.DeliveryFee, request.ParentZoneId);

		if (zone.IsError)
		{
			return zone.Errors;
		}

		context.Zones.Add(zone.Value);
		await context.SaveChangesAsync(cancellationToken);

		return zone.Value.Id;
	}
}
