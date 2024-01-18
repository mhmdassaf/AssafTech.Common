namespace AssafTech.Common.Models;

public class ErrorModel
{
    public string? Code { get; set; }
    public string? Message { get; set; }

    public ErrorModel()
    {
            
    }

    public ErrorModel(string code, string message)
    {
        Code = code;
        Message = message;
    }
}
