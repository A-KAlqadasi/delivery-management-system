using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Modules.Common.Infrastructure.Authentication;

internal sealed class JwtBearerConfigureOptions(
	IConfiguration configuration)
	: IConfigureNamedOptions<JwtBearerOptions>
{
	private const string ConfigurationSectionName = "JwtSettings";

	public void Configure(string? name, JwtBearerOptions options)
	{
		IConfigurationSection section =
			configuration.GetSection(ConfigurationSectionName);

		string secret = section["Secret"]!;

		options.SaveToken = true;
		options.MapInboundClaims = false;

		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidIssuer = section["Issuer"],

			ValidateAudience = true,
			ValidAudience = section["Audience"],

			ValidateLifetime = true,

			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(secret)),

			ClockSkew = TimeSpan.FromMinutes(1)
		};
	}

	public void Configure(JwtBearerOptions options)
	{
		Configure(Options.DefaultName, options);
	}
}
