using System.Data.Common;

using Npgsql;

namespace Modules.Common.Infrastructure.Data;

internal sealed class DBConnectionFactory(NpgsqlDataSource dataSource) : IDbConnectionFactory
{
	public async ValueTask<DbConnection> OpenConnectionAsync()
	{
		return await dataSource.OpenConnectionAsync();
	}
}
