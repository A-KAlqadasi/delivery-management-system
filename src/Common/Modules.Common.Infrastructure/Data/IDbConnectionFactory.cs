using System.Data.Common;

namespace Modules.Common.Infrastructure.Data;

public interface IDbConnectionFactory
{
	ValueTask<DbConnection> OpenConnectionAsync();
}
