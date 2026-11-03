using System.ComponentModel.DataAnnotations;

namespace Modules.Users.Infrastructure.Identity;

public class JwtSettings
{
	public const string SectionName = "JwtSettings";

	[Required, MinLength(32)] // Secret must be at least 32 chars for security
	public string Secret { get; set; } = string.Empty;

	[Range(1, 1440)] // Between 1 minute and 1 day
	public int TokenExpirationInMinutes { get; set; }

	[Required]
	public string Issuer { get; set; } = string.Empty;

	[Required]
	public string Audience { get; set; } = string.Empty;
}
