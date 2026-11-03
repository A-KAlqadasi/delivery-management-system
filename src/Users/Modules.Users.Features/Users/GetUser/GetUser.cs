using Microsoft.EntityFrameworkCore;

using Modules.Common.Application.MediatR;
using Modules.Common.Domain.Results;
using Modules.Users.Domain.Users;
using Modules.Users.Features.Abstractions.Data;
using Modules.Users.Features.Users.Shared.Responses;

namespace Modules.Users.Features.Users.GetUser;

public sealed record GetUserQuery(Guid UserId) : IRequest<Result<UserResponse>>;

internal sealed class GetUserQueryHandler(IUsersDbContext context)
	: IRequestHandler<GetUserQuery, Result<UserResponse>>
{
	public async Task<Result<UserResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
	{
		var user = await context.Users
			.Where(u => u.Id == request.UserId)
			.Select(u => new UserResponse(
				u.Id,
				u.Email,
				u.FirstName,
				u.LastName))
			.FirstOrDefaultAsync(cancellationToken);

		// UserResponse? user = await userRepository.GetUserByIdAsync(request.UserId, cancellationToken);

		return user ?? (Result<UserResponse>)UserErrors.NotFound(request.UserId);
	}
}
