namespace Modules.Users.Features.Users.Shared.Responses;

public sealed record UserResponse(Guid Id, string Email, string FirstName, string LastName);
