
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

using Modules.Common.Application.Clock;
using Modules.Common.Domain;

namespace AlatrafClinic.Infrastructure.Data.Interceptors;

public class AuditableEntityInterceptor(IDateTimeProvider dateTime) : SaveChangesInterceptor
{

	private readonly IDateTimeProvider _dateTime = dateTime;

	public override InterceptionResult<int> SavingChanges(
		DbContextEventData eventData,
		InterceptionResult<int> result)
	{
		UpdateEntities(eventData.Context);
		return base.SavingChanges(eventData, result);
	}

	public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
		DbContextEventData eventData,
		InterceptionResult<int> result,
		CancellationToken cancellationToken = default)
	{
		UpdateEntities(eventData.Context);
		return base.SavingChangesAsync(eventData, result, cancellationToken);
	}

	private void UpdateEntities(DbContext? context)
	{
		if (context is null)
		{
			return;
		}

		var utcNow = _dateTime.UtcNow;


		foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
		{
			if (entry.State == EntityState.Added)
			{
				entry.Entity.CreatedAtUtc = utcNow;
			}

			if (entry.State is EntityState.Added or EntityState.Modified || entry.HasChangedOwnedEntities())
			{
				entry.Entity.UpdatedAtUtc = utcNow;
			}

			foreach (var targetEntry in entry.References
											.Select(t => t.TargetEntry)
											.Where(e => e != null))
			{
				if (targetEntry?.Entity is IAuditableEntity owned &&
					targetEntry.State is EntityState.Added or EntityState.Modified)
				{
					if (targetEntry.State == EntityState.Added)
					{
						owned.CreatedAtUtc = utcNow;
					}

					owned.UpdatedAtUtc = utcNow;
				}
			}
		}

	}
}

public static class EntityEntryExtensions
{
	public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
		entry.References.Any(r =>
			r.TargetEntry?.Metadata.IsOwned() == true &&
			(r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}
