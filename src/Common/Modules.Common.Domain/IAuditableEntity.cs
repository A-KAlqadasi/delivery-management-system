namespace Modules.Common.Domain;

public interface IAuditableEntity
{
	string? CreatedBy { get; set; }
	DateTime CreatedAtUtc { get; set; }
	string? UpdatedBy { get; set; }
	DateTime? UpdatedAtUtc { get; set; }
}

public abstract class AuditableEntity : Entity, IAuditableEntity
{
	public string? CreatedBy { get; set; }
	public DateTime CreatedAtUtc { get; set; }
	public string? UpdatedBy { get; set; }
	public DateTime? UpdatedAtUtc { get; set; }
}
