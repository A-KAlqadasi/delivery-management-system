using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Modules.Users.Infrastructure.Database;

public class UsersDbContextFactory
		: IDesignTimeDbContextFactory<UsersDbContext>
{
	public UsersDbContext CreateDbContext(string[] args)
	{


		DbContextOptionsBuilder<UsersDbContext> optionsBuilder = new DbContextOptionsBuilder<UsersDbContext>();

		optionsBuilder.UseNpgsql("Host=evently.database;Port=5432;Database=evently;Username=postgres;Password=postgres;Include Error Detail=true", npgsqlOptions => npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Users))
		 .UseSnakeCaseNamingConvention()
		 .AddInterceptors();

		return new UsersDbContext(optionsBuilder.Options);
	}
}
