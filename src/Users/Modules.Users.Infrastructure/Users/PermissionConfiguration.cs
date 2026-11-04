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

		builder.HasData(Permission.GetUser, Permission.CreateUser, Permission.ReadZone, Permission.CreateZone, Permission.UpdateZone, Permission.DeleteZone);

		builder
			.HasMany<Role>()
			.WithMany(r => r.Permissions)
			.UsingEntity(joinBuilder =>
			{
				joinBuilder.ToTable("role_permissions");

				joinBuilder.HasData(
					// Member permissions
					CreateRolePermission(Role.Admin, Permission.GetUser),
					CreateRolePermission(Role.Admin, Permission.CreateUser),
					CreateRolePermission(Role.Admin, Permission.ReadZone),
					CreateRolePermission(Role.Admin, Permission.CreateZone),
					CreateRolePermission(Role.Admin, Permission.UpdateZone),
					CreateRolePermission(Role.Admin, Permission.DeleteZone),

					CreateRolePermission(Role.Courier, Permission.GetUser),
					CreateRolePermission(Role.Dispatcher, Permission.GetUser)

					);
			});
	}

	private static object CreateRolePermission(Role role, Permission permission)
	{
		return new
		{
			RoleName = role.Name,
			PermissionsCode = permission.Code
		};
	}
}
