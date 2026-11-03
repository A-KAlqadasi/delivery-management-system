namespace Modules.Users.Features.Abstractions.Identity;

public class TokenResponse
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime ExpiresOnUtc { get; set; }
}
