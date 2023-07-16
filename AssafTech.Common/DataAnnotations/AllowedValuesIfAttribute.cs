namespace AssafTech.Common.DataAnnotations;

public class AllowedValuesIfAttribute : ValidationAttribute
{
	private readonly object[] _allowedValues;
	private readonly string _otherName;
	private readonly Operator _operator;
	private readonly object? _otherValue;

	public AllowedValuesIfAttribute(string otherPropertyName, Operator op, object? otherPropertyValue, params object[] allowedValues)
    {
		_allowedValues = allowedValues;
		_otherName = otherPropertyName;
		_operator = op;
		_otherValue = otherPropertyValue;
	}

	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		var otherName = validationContext.ObjectInstance.GetType().GetProperty(_otherName);
		if (otherName == null) { return new ValidationResult(ErrorMessage); }

		var otherValue = otherName.GetValue(validationContext.ObjectInstance, null);

		var otherValueAsString = otherValue?.ToString();
		if (otherValueAsString != null && string.IsNullOrWhiteSpace(otherValueAsString)) otherValueAsString = null;

		var _otherValueAsString = _otherValue?.ToString();
		if (_otherValueAsString != null && string.IsNullOrWhiteSpace(_otherValueAsString)) _otherValueAsString = null;

		bool isValid = false;
		switch (_operator)
		{
			case Operator.Equal:
				isValid = (_otherValueAsString == null && otherValueAsString == null) || 
					       _otherValueAsString == otherValueAsString;
				break;
			case Operator.NotEqual:
				isValid = (_otherValueAsString == null && !string.IsNullOrWhiteSpace(otherValueAsString?.ToString())) || 
					      (_otherValueAsString != null && otherValueAsString == null) || 
						   _otherValueAsString != otherValueAsString;
				break;
			case Operator.GreaterThan:
				isValid = _otherValueAsString != null &&
						  otherValueAsString != null &&
						  double.TryParse(_otherValueAsString.ToString(), out double _otherValueResult) &&
						  double.TryParse(otherValueAsString.ToString(), out double otherValueResult) &&
						  _otherValueResult > otherValueResult;
				break;
			case Operator.GreaterThanOrEqual:
				isValid = _otherValueAsString != null &&
						  otherValueAsString != null && 
						  double.TryParse(_otherValueAsString.ToString(), out double _otherValueResult1) &&
						  double.TryParse(otherValueAsString.ToString(), out double otherValueResult1) &&
						  _otherValueResult1 >= otherValueResult1;
				break;
			case Operator.LessThan:
				isValid = _otherValueAsString != null &&
						  otherValueAsString != null && 
						  double.TryParse(_otherValueAsString.ToString(), out double _otherValueResult2) &&
						  double.TryParse(otherValueAsString.ToString(), out double otherValueResult2) &&
						  _otherValueResult2 < otherValueResult2;
				break;
			case Operator.LessThanOrEqual:
				isValid = _otherValueAsString != null &&
						  otherValueAsString != null && 
						  double.TryParse(_otherValueAsString.ToString(), out double _otherValueResult3) &&
						  double.TryParse(otherValueAsString.ToString(), out double otherValueResult3) &&
						  _otherValueResult3 <= otherValueResult3;
				break;
			case Operator.Contain:
				isValid = _otherValueAsString != null &&
						  otherValueAsString != null &&
						  _otherValueAsString.Contains(otherValueAsString);
				break;
			case Operator.DoesNotContain:
				isValid = _otherValueAsString != null &&
						  _otherValueAsString != null &&
						 !_otherValueAsString.Contains(_otherValueAsString);
				break;
		}

		if(isValid)
		{
			var valueAsString = value?.ToString();
			if (string.IsNullOrWhiteSpace(valueAsString))
			{
				return new ValidationResult($"{validationContext.MemberName} {General.IsRequired}");
			}
			else
			{
				foreach (var allowedValue in _allowedValues)
				{
					if (allowedValue.ToString() == valueAsString)
						return ValidationResult.Success;
				}
				return new ValidationResult(ErrorMessage);
			}
		}
		else
		{
			return ValidationResult.Success;
		}
	}
}
