namespace AssafTech.Common.Enums;

public enum InvoiceStatus
{
	[Description("Invoice Pending")]
	Pending = 7,

	[Description("Invoice Partially Paid")]
	PartiallyPaid = 8,

	[Description("Invoice Paid")]
	Paid = 9,
}
