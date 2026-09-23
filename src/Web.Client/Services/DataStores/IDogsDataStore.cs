using DogAppBlazor.Contracts.Dogs;

namespace DogAppBlazor.Web.Client.Services.DataStores;

/// <summary>
/// Klientska cache zoznamu psov. Zoznam sa meni zriedka, preto sa nacita raz
/// a po kazdej zmene sa invaliduje volanim <see cref="Clear"/>.
/// </summary>
public interface IDogsDataStore
{
	Task<List<DogListItemDto>> GetAllAsync(CancellationToken cancellationToken = default);

	void Clear();
}
