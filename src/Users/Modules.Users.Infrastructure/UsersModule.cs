using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;

using Modules.Common.Infrastructure;
using Modules.Common.Api.Extensions;
using Modules.Users.Features.Abstractions.Data;
using Modules.Users.Infrastructure.Authorization;
using Modules.Users.Infrastructure.Database;
using Modules.Common.Application;
using Modules.Common.Application.Authorization;
using Microsoft.EntityFrameworkCore.Migrations;
using Modules.Users.Domain.Users;
using Modules.Users.Infrastructure.Identity;
using Modules.Users.Features.Abstractions.Identity;

namespace Modules.Users.Infrastructure;


public static class UsersModule
{

	public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddOptionsWithValidateOnStart<JwtSettings>()
	   .BindConfiguration(JwtSettings.SectionName)
	   .ValidateDataAnnotations();

		// services.AddDomainEventHandlers();

		// services.AddIntegrationEventHandlers();
		// Commong API configurations
		services.AddEndpoints();

		// Common Application configurations
		services.AddApplication([Features.AssemblyReference.Assembly]);

		services.AddUsersInfrastructure(configuration);

		return services;
	}

	private static IServiceCollection AddUsersInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<IPermissionService, PermissionService>();

		services
	.AddIdentityCore<User>(options =>
	{
		options.User.RequireUniqueEmail = true;

		options.Password.RequiredLength = 8;
		options.Password.RequireDigit = true;
		options.Password.RequireUppercase = true;
		options.Password.RequireLowercase = true;
		options.Password.RequireNonAlphanumeric = false;

	})
	.AddEntityFrameworkStores<UsersDbContext>()
	.AddSignInManager()
	.AddDefaultTokenProviders();


		string databaseConnectionString = configuration.GetConnectionString("Database")!;
		// Common infrastructure configurations
		services.AddCoreInfrastructure();

		services.AddDbContext<UsersDbContext>((sp, options) =>
			options.UseSqlServer(databaseConnectionString,
			sqlServerOptions => sqlServerOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Users))
			.UseUpperCaseNamingConvention()
		);

		services.AddScoped<IUsersDbContext>(c => c.GetRequiredService<UsersDbContext>());
		services.AddScoped<UsersDatabaseInitializer>();

		services.AddScoped<ITokenProvider, TokenProvider>();


		return services;
	}

}
