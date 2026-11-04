namespace Delivery.Domain;

public static class Permissions
{
	public static string ReadZone => "zones:read";
	public static string CreateZone => "zones:create";
	public static string UpdateZone => "zones:update";
	public static string DeleteZone => "zones:delete";
}
