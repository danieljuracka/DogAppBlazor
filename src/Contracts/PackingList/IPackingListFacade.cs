using Havit.ComponentModel;

namespace DogAppBlazor.Contracts.PackingList;

[ApiContract]
public interface IPackingListFacade
{
	/// <summary>
	/// Vráti všetky položky zoradené podľa kategórie a názvu. Pri úplne prvom načítaní
	/// zoznam naplní predvolenými položkami.
	/// </summary>
	Task<List<PackingItemDto>> GetItemsAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Založí novú položku (Id == 0) alebo aktualizuje existujúcu. Vracia Id položky.
	/// </summary>
	Task<Dto<int>> UpdateItemAsync(PackingItemDto packingItemDto, CancellationToken cancellationToken = default);

	/// <summary>
	/// Označí položku ako zbalenú alebo nezbalenú.
	/// </summary>
	Task SetItemPackedAsync(PackingItemPackedDto packingItemPackedDto, CancellationToken cancellationToken = default);

	/// <summary>
	/// Označí všetky položky ako zbalené (true) alebo zruší všetky označenia (false).
	/// </summary>
	Task SetAllPackedAsync(Dto<bool> isPacked, CancellationToken cancellationToken = default);

	/// <summary>
	/// Zmaže položku.
	/// </summary>
	Task DeleteItemAsync(Dto<int> id, CancellationToken cancellationToken = default);

	/// <summary>
	/// Vygeneruje Excel (.xlsx) so všetkými položkami zoznamu.
	/// </summary>
	Task<PackingListExportDto> ExportToExcelAsync(CancellationToken cancellationToken = default);
}
