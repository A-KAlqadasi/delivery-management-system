using Microsoft.EntityFrameworkCore;

using Modules.Users.Domain.Users;

namespace Modules.Users.Features.Abstractions.Data;

public interface IUsersDbContext
{
	DbSet<User> Users { get; }
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
