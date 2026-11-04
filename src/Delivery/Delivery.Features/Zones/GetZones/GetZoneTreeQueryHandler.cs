using Delivery.Features.Abstractions;

using Microsoft.EntityFrameworkCore;

using Modules.Common.Application.MediatR;
using Modules.Common.Domain.Results;

namespace Delivery.Features.Zones.GetZones;

internal sealed record GetZoneTreeQuery : IRequest<Result<List<GetZonesResponse>>>;

internal sealed class GetZoneTreeQueryHandler(IDeliveryDbContext context)
	: IRequestHandler<GetZoneTreeQuery, Result<List<GetZonesResponse>>>
{
	public async Task<Result<List<GetZonesResponse>>> Handle(
		GetZoneTreeQuery request,
		CancellationToken cancellationToken)
	{
		var zones = await context.Zones
			.AsNoTracking()
			.Select(z => new { z.Id, z.Name, z.DeliveryFee, z.ParentZoneId })
			.ToListAsync(cancellationToken);

		var byParent = zones
			.GroupBy(z => z.ParentZoneId ?? Guid.Empty)
			.ToDictionary(g => g.Key, g => g.ToList());

		List<GetZonesResponse> Build(Guid parentId) =>
			byParent.TryGetValue(parentId, out var children)
				? children
					.Select(c => new GetZonesResponse(
						c.Id, c.Name, c.DeliveryFee, Build(c.Id)))
					.ToList()
				: new List<GetZonesResponse>();

		return Build(Guid.Empty);
	}
}
