using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Modules.Users.Domain.Users;

namespace Modules.Users.Infrastructure.Users;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
	public void Configure(EntityTypeBuilder<Permission> builder)
	{
		builder.ToTable("permissions");

		builder.HasKey(p => p.Code);

		builder.Property(p => p.Code).HasMaxLength(100);

		builder.HasData(Permission.GetUser, Permission.ModifyUser, Permission.GetEvents, Permission.SearchEvents, Permission.ModifyEvents, Permission.GetTicketTypes, Permission.ModifyTicketTypes, Permission.GetCategories, Permission.ModifyCategories, Permission.GetCart, Permission.AddToCart, Permission.RemoveFromCart, Permission.GetOrders, Permission.CreateOrder, Permission.GetTickets, Permission.CheckInTicket, Permission.GetEventStatistics);

		builder
			.HasMany<Role>()
			.WithMany(r => r.Permissions)
			.UsingEntity(joinBuilder =>
			{
				joinBuilder.ToTable("role_permissions");

				joinBuilder.HasData(
					// Member permissions
					CreateRolePermission(Role.Courier, Permission.GetUser),
					CreateRolePermission(Role.Courier, Permission.ModifyUser),
					CreateRolePermission(Role.Courier, Permission.SearchEvents),
					CreateRolePermission(Role.Courier, Permission.GetTicketTypes),
					CreateRolePermission(Role.Courier, Permission.GetCart),
					CreateRolePermission(Role.Courier, Permission.AddToCart),
					CreateRolePermission(Role.Courier, Permission.RemoveFromCart),
					CreateRolePermission(Role.Courier, Permission.GetOrders),
					CreateRolePermission(Role.Courier, Permission.CreateOrder),
					CreateRolePermission(Role.Courier, Permission.GetTickets),
					CreateRolePermission(Role.Courier, Permission.CheckInTicket),
					// Admin permissions
					CreateRolePermission(Role.Admin, Permission.GetUser),
					CreateRolePermission(Role.Admin, Permission.ModifyUser),
					CreateRolePermission(Role.Admin, Permission.GetEvents),
					CreateRolePermission(Role.Admin, Permission.SearchEvents),
					CreateRolePermission(Role.Admin, Permission.ModifyEvents),
					CreateRolePermission(Role.Admin, Permission.GetTicketTypes),
					CreateRolePermission(Role.Admin, Permission.ModifyTicketTypes),
					CreateRolePermission(Role.Admin, Permission.GetCategories),
					CreateRolePermission(Role.Admin, Permission.ModifyCategories),
					CreateRolePermission(Role.Admin, Permission.GetCart),
					CreateRolePermission(Role.Admin, Permission.AddToCart),
					CreateRolePermission(Role.Admin, Permission.RemoveFromCart),
					CreateRolePermission(Role.Admin, Permission.GetOrders),
					CreateRolePermission(Role.Admin, Permission.CreateOrder),
					CreateRolePermission(Role.Admin, Permission.GetTickets),
					CreateRolePermission(Role.Admin, Permission.CheckInTicket),
					CreateRolePermission(Role.Admin, Permission.GetEventStatistics));
			});
	}

	private static object CreateRolePermission(Role role, Permission permission)
	{
		return new
		{
			RoleName = role.Name,
			PermissionCode = permission.Code
		};
	}
}
