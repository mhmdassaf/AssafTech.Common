namespace AssafTech.Common.DataAnnotations;

public class LessThanAttribute : ValidationAttribute
{
	private readonly string _otherProperty;

	public LessThanAttribute(string otherProperty)
    {
		_otherProperty = otherProperty;
	}

	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		var otherPropertyName = validationContext.ObjectInstance.GetType().GetProperty(_otherProperty);
		if (otherPropertyName == null) { return new ValidationResult(ErrorMessage); }

		var otherPropertyValue = otherPropertyName.GetValue(validationContext.ObjectInstance, null);

		var isValid = value != null &&
					  otherPropertyValue != null &&
					  double.TryParse(otherPropertyValue?.ToString(), out double otherPropertyValueAsNb) &&
					  double.TryParse(value?.ToString(), out double valuesNb) &&
					  valuesNb < otherPropertyValueAsNb;

		return isValid ? ValidationResult.Success : new ValidationResult($"the field {validationContext.MemberName} is greater than the allowed value");
	}

}
