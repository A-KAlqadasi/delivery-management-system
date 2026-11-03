namespace Modules.Users.Domain.Users;

public sealed class Permission
{
	public static readonly Permission GetUser = new("users:read");
	public static readonly Permission CreateUser = new("users:create");

	public Permission(string code)
	{
		Code = code;
	}

	public string Code { get; }
}
