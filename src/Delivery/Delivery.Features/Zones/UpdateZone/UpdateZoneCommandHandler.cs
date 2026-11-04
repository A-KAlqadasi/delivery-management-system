using Delivery.Domain;
using Delivery.Domain.Zones;
using Delivery.Features.Abstractions;

using Microsoft.EntityFrameworkCore;

using Modules.Common.Application.MediatR;
using Modules.Common.Domain.Results;

namespace Delivery.Features.Zones.UpdateZone;

internal sealed record UpdateZoneCommand(
	Guid Id,
	string Name,
	decimal? DeliveryFee,
	Guid? ParentZoneId) : IRequest<Result<Updated>>;

internal sealed class UpdateZoneCommandHandler(IDeliveryDbContext context)
	: IRequestHandler<UpdateZoneCommand, Result<Updated>>
{
	public async Task<Result<Updated>> Handle(
		UpdateZoneCommand request,
		CancellationToken cancellationToken)
	{
		var zone = await context.Zones
			.Include(z => z.ParentZone)
			.FirstOrDefaultAsync(z => z.Id == request.Id, cancellationToken);

		if (zone is null)
		{
			return DeliveryErrors.ZoneNotFound;
		}

		Zone? newParent = null;
		if (request.ParentZoneId is Guid parentId)
		{
			if (parentId == request.Id)
			{
				return DeliveryErrors.ZoneCannotBeItsOwnParent;
			}

			newParent = await context.Zones
				.Include(z => z.ParentZone)
				.FirstOrDefaultAsync(z => z.Id == parentId, cancellationToken);

			if (newParent is null)
			{
				return DeliveryErrors.ParentZoneNotFound;
			}
		}

		var assignResult = zone.AssignParent(newParent);
		if (assignResult.IsError)
		{
			return assignResult.Errors;
		}

		zone.Rename(request.Name);
		zone.ChangeDeliveryFee(request.DeliveryFee);

		await context.SaveChangesAsync(cancellationToken);
		return Result.Updated;
	}
}
