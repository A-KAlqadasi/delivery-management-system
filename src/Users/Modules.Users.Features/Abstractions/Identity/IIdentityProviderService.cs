using Modules.Common.Domain.Results;
using Modules.Users.Domain.Users;

namespace Modules.Users.Features.Abstractions.Identity;

public interface IIdentityProviderService
{
	Task<Result<string>> RegisterUserAsync(UserModel user, CancellationToken cancellationToken = default);
	Task<Result<UserDetailsDto>> AuthenticateAsync(string userName, string password);

	Task<Result<RefreshToken>> GetRefreshTokenAsync(string refreshToken, string userId);

}
