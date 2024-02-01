namespace AssafTech.Common.Models;

public class ErrorModel
{
    public int Code { get; set; }
    public string? Message { get; set; }

    public ErrorModel()
    {
            
    }

    public ErrorModel(int code, string message)
    {
        Code = code;
        Message = message;
    }
}
