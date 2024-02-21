namespace AssafTech.Common.Models;

public class EndPointModel
{
    public string? ServiceName { get; set; }
    public required string ControllerName { get; set; }
    public required string ActionName { get; set; }
    public string? QueryParams { get; set; }
    public string? PathParams { get; set; }
}
