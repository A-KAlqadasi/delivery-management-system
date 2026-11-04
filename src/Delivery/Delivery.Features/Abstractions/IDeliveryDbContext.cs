using Delivery.Domain.Zones;

using Microsoft.EntityFrameworkCore;

namespace Delivery.Features.Abstractions;

public interface IDeliveryDbContext
{
	DbSet<Zone> Zones { get; }
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
