namespace AssafTech.Common.Entities;

public class BaseEntity
{
	public long Id { get; set; }

    public DateTime CreateDate { get; set; }

	[Required]
	public required string CreatedBy { get; set; }

	public DateTime UpdateDate { get; set; }

	public string? UpdatedBy { get; set; }

	public DateTime DeleteDate { get; set; }

	public string? DeletedBy { get; set; }
}
