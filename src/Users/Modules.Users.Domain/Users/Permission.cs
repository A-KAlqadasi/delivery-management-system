namespace Modules.Users.Domain.Users;

public sealed class Permission
{
	public static readonly Permission GetUser = new("users:read");
	public static readonly Permission CreateUser = new("users:create");

	public static readonly Permission ReadZone = new("zones:read");
	public static readonly Permission CreateZone = new("zones:create");
	public static readonly Permission UpdateZone = new("zones:update");
	public static readonly Permission DeleteZone = new("zones:delete");


	public Permission(string code)
	{
		Code = code;
	}

	public string Code { get; }
}
