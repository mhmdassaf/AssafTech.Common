namespace AssafTech.Common.Controllers;

[ApiController, Route("api/[controller]/[action]"), Authorize]
public abstract class BaseController : ControllerBase
{
	public ResponseModel ResponseModel { get; }

	public BaseController()
    {
        var responseModel = new ResponseModel();
        ResponseModel = responseModel;
    }
}
