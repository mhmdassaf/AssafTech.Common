namespace AssafTech.Common.Dtos;

public class ListDto
{
	#region Private
	private string? search;
	#endregion

	[DefaultValue(null)]
    public string? OrderBy { get; set; }

	[AllowedValuesIf(nameof(OrderBy), Operator.NotEqual, null, Sort.Ascending, Sort.Descending)]
	[DefaultValue(Sort.Descending)]
	public string? SortDirection { get; set; }

	public string? Search
	{
		get { return search; }
		set { search = value?.ToLower(); }
	}

	[DefaultValue(1)]
	public required int PageIndex { get; set; }

	[DefaultValue(10)]
	public required int PageSize { get; set; }
}
