namespace AssafTech.Common.Helpers;

public static class ValidatorHelper
{
    public static IEnumerable<ErrorModel> MapErrors(List<ValidationFailure> errors)
    {
        return errors.Select(err =>
        {
            if(int.TryParse(err.ErrorCode, out int code))
            {
                return new ErrorModel { Code = code, Message = err.ErrorMessage };
            }
            else
            {
                return new ErrorModel 
                { 
                    Code = 0, 
                    Message = $"Unable to parse the errorCode '{err.ErrorCode}' from string to integer. Please check your validator errorCode must be integer format" 
                };
            }
        });
    }
}
