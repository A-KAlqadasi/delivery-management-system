using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Modules.Users.Infrastructure.Database;

internal class UsersDbContextFactory
		: IDesignTimeDbContextFactory<UsersDbContext>
{
	public UsersDbContext CreateDbContext(string[] args)
	{


		DbContextOptionsBuilder<UsersDbContext> optionsBuilder = new DbContextOptionsBuilder<UsersDbContext>();

		optionsBuilder.UseSqlServer("Server=Abu-Ghaith\\AMSQLSERVER;Database=DeliveryManagementDb;User Id=sa;Password=isa123456;TrustServerCertificate=True;", sqlServerOptions => sqlServerOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Users))
		 .UseUpperCaseNamingConvention()
		 .AddInterceptors();

		return new UsersDbContext(optionsBuilder.Options);
	}
}
