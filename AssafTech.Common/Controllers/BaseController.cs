namespace AssafTech.Common.Controllers;

[ApiController, Route("api/[controller]/[action]"), Authorize]
public abstract class BaseController : ControllerBase
{
}
