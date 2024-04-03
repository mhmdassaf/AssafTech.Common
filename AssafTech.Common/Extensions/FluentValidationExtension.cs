namespace AssafTech.Common.Extensions;

public static class FluentValidationExtension
{
    public static IEnumerable<ErrorModel> ToResponseModelErrors(this List<ValidationFailure> errors)
    {
        int result;
        return errors.Select((ValidationFailure err) => int.TryParse(err.ErrorCode, out result) ? new ErrorModel
        {
            Code = result,
            Message = err.ErrorMessage
        } : new ErrorModel
        {
            Code = 0,
            Message = "Unable to parse the errorCode '" + err.ErrorCode + "' from string to integer. Please check your validator errorCode must be integer format"
        });
    }
}
