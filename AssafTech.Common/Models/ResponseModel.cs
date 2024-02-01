namespace AssafTech.Common.Models;

public class ResponseModel
{
    public ResponseModel()
    {
        Errors = new List<ErrorModel>();
	}

    public bool Succeeded => !Errors.Any();
    public List<ErrorModel> Errors { get; set; }
    public int Code { get; set; }
    public string? Message { get; set; }
    public object? Result { get; set; }
}
