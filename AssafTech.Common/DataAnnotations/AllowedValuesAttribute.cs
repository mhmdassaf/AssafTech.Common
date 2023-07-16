namespace AssafTech.Common.DataAnnotations;

public class AllowedValuesAttribute : ValidationAttribute
{
	private readonly object[] _allowedValues;

	public AllowedValuesAttribute(params object[] allowedValues)
	{
		_allowedValues = allowedValues;
	}
	public override bool IsValid(object? value)
	{
		foreach (var allowedValue in _allowedValues)
		{
			if(value != null && allowedValue.ToString() == value.ToString()) 
				return true;
		}
		return false;
	}
}

