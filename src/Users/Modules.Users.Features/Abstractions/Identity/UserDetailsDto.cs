namespace Modules.Users.Features.Abstractions.Identity;

public class UserDetailsDto
{
    public string UserId { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = default!;
    public IReadOnlyList<string> Permissions { get; set; } = [];
}
