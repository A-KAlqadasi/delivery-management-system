using Delivery.Domain.Zones;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Delivery.Infrastructure.Zones;

public class ZoneConfiguration : IEntityTypeConfiguration<Zone>
{
	public void Configure(EntityTypeBuilder<Zone> builder)
	{
		builder.ToTable("Zones");

		builder.HasKey(z => z.Id);

		builder.Property(z => z.Id)
			   .ValueGeneratedNever();

		builder.Property(z => z.Name)
			   .IsRequired()
			   .HasMaxLength(200);

		builder.Property(z => z.DeliveryFee)
			   .HasPrecision(18, 2);

		builder.HasOne(z => z.ParentZone)
			   .WithMany(z => z.Children)
			   .HasForeignKey(z => z.ParentZoneId)
			   .OnDelete(DeleteBehavior.Restrict);

		builder.HasIndex(z => z.ParentZoneId);

	}
}
