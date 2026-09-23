using Havit.ComponentModel;

namespace DogAppBlazor.Contracts.Dogs;

[ApiContract]
public interface IDogFacade
{
	/// <summary>
	/// Vrati zoznam vsetkych psov zoradeny podla mena.
	/// </summary>
	Task<List<DogListItemDto>> GetDogsAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Vrati profil psa. Ak pes neexistuje, vrati null.
	/// </summary>
	Task<DogDto> GetDogAsync(Dto<int> id, CancellationToken cancellationToken = default);

	/// <summary>
	/// Zalozi noveho psa (Id == 0) alebo aktualizuje existujuceho. Vracia Id zaznamu.
	/// </summary>
	Task<Dto<int>> UpdateDogAsync(DogDto dogDto, CancellationToken cancellationToken = default);
}
