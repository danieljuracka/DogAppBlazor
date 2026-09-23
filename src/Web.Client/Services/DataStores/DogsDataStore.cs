using DogAppBlazor.Contracts.Dogs;

namespace DogAppBlazor.Web.Client.Services.DataStores;

public class DogsDataStore(IDogFacade dogFacade) : IDogsDataStore
{
	private readonly IDogFacade _dogFacade = dogFacade;
	private List<DogListItemDto> _dogs;

	public async Task<List<DogListItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		_dogs ??= await _dogFacade.GetDogsAsync(cancellationToken);
		return _dogs;
	}

	public void Clear()
	{
		_dogs = null;
	}
}
