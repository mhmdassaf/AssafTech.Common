namespace AssafTech.Common.Enums;

public enum InvoiceStatus
{
	[Description("غير مدفوعة")]
	Pending = 7,

	[Description("مدفوعة جزئيا")]
	PartiallyPaid = 8,

	[Description("مدفوعة")]
	Paid = 9,
}
