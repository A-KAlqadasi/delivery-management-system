namespace Modules.Users.Domain.Users;

public sealed class Role
{
	private readonly List<Permission> _permissions = [];

	public static readonly Role Admin = new("Admin");
	public static readonly Role Dispatcher = new("Dispatcher");
	public static readonly Role Courier = new("Courier");



	private Role(string name)
	{
		Name = name;
	}

	private Role()
	{
	}

	public string Name { get; } = default!;
	public IReadOnlyCollection<Permission> Permissions =>
	  _permissions.AsReadOnly();
}
