using Delivery.Domain.Zones;
using Delivery.Features.Abstractions;

using Microsoft.EntityFrameworkCore;


namespace Delivery.Infrastructure.Database;

internal sealed class DeliveryDbContext(
	DbContextOptions<DeliveryDbContext> options)
	: DbContext(options), IDeliveryDbContext
{
	public DbSet<Zone> Zones => Set<Zone>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.HasDefaultSchema(Schemas.Delivery);
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(DeliveryDbContext).Assembly);
	}
}
