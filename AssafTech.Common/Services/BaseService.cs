namespace AssafTech.Common.Services;

public abstract class BaseService
{
	public IRepository _repository { get; }
	public IMapper _mapper { get; }
	public HttpContext? _httpContext { get; }

	public BaseService(IRepository repository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
		_repository = repository;
		_mapper = mapper;
		_httpContext = httpContextAccessor.HttpContext;
	}
}
