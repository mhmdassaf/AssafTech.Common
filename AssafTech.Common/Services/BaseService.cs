namespace AssafTech.Common.Services;

public abstract class BaseService
{
	protected readonly IRepository _repository;
	protected readonly IMapper _mapper;
	protected readonly HttpContext? _httpContext;

	public string UserId
	{
		get {
			string? userId = _httpContext?.User.FindFirstValue(JwtClaimTypes.Subject);
			if (string.IsNullOrWhiteSpace(userId))
			{
				throw new UnauthorizedAccessException($"{nameof(UserId)} {General.IsNull}");
			}
			return userId; 
		}
	}
	public ResponseModel ResponseModel { get; }

	public BaseService(IRepository repository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
		_repository = repository;
		_mapper = mapper;
		_httpContext = httpContextAccessor.HttpContext;
		var responseModel = new ResponseModel();
		ResponseModel = responseModel;
	}
}
