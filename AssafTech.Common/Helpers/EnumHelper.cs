namespace AssafTech.Common.Helpers;

public static class EnumHelper
{
	public static string? ToDescription(this Enum value)
	{
		var fieldInfo = value.GetType().GetField(value.ToString());
		if (fieldInfo == null) return null;
		var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
		return attributes.Length > 0 ? attributes[0].Description : value.ToString();
	}
}
