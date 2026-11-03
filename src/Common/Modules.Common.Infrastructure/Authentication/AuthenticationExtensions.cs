using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace Modules.Common.Infrastructure.Authentication;
/// <summary>
/// Extension methods for configuring authentication services in the application. The AddAuthenticationInternal method sets up JWT bearer authentication and authorization services, allowing the application to authenticate and authorize users based on JWT tokens. It also configures the necessary options for JWT bearer authentication using the JwtBearerConfigureOptions class, ensuring that the authentication mechanism is properly set up and ready to be used in the application. This extension method can be called during the service registration phase to add authentication capabilities to the application, enabling secure access control and user authentication based on JWT tokens.
/// </summary>
internal static class AuthenticationExtensions
{
	internal static IServiceCollection AddAuthenticationInternal(this IServiceCollection services)
	{
		services.AddAuthorization();

		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

		services.AddHttpContextAccessor();

		services.ConfigureOptions<JwtBearerConfigureOptions>();

		return services;
	}
}
