namespace Modules.Users.Features.Abstractions.Identity;

public sealed record UserDetailsDto(
	Guid UserId,
	string Email,
	IReadOnlyCollection<string> Roles,
	IReadOnlyCollection<string> Permissions);
