using System.Net;

namespace AssafTech.Common.Models;

public class ResponseModel
{
    public ResponseModel()
    {
        Errors = new List<ErrorModel>();
    }

    public bool Succeeded => !Errors.Any();
    public HttpStatusCode HttpStatusCode { get; set; }
    public List<ErrorModel> Errors { get; set; }
    public object? Result { get; set; }
}
