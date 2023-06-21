namespace AssafTech.Common.Services;

public abstract class BaseService
{
	public IRepository _repository { get; }
	public IMapper _mapper { get; }

	public BaseService(IRepository repository, IMapper mapper)
    {
		_repository = repository;
		_mapper = mapper;
	}
}
