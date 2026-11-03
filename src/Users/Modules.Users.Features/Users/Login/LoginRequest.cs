using Modules.Common.Application.MediatR;
using Modules.Common.Domain.Results;
using Modules.Users.Features.Abstractions.Identity;

namespace Modules.Users.Features.Users.Login;

public record LoginRequest(
	string Email,
	string Password) : IRequest<Result<TokenResponse>>;
