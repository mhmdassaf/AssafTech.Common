using System.Net;

namespace AssafTech.Common.Models;

public class ResponseModel
{
    public bool Succeeded => Errors == null || !Errors.Any();
    public HttpStatusCode HttpStatusCode { get; set; }
    public ErrorModel[]? Errors { get; set; }
    public object? Result { get; set; }
}
