using Havit.ComponentModel;

namespace DogAppBlazor.Contracts.Records;

[ApiContract]
public interface IRecordFacade
{
	/// <summary>
	/// Vráti záznamy podľa filtra, zoradené od najnovšieho.
	/// </summary>
	Task<List<RecordListItemDto>> GetRecordsAsync(RecordFilterDto filter, CancellationToken cancellationToken = default);

	/// <summary>
	/// Vráti záznam. Ak neexistuje, vráti null.
	/// </summary>
	Task<RecordDto> GetRecordAsync(Dto<int> id, CancellationToken cancellationToken = default);

	/// <summary>
	/// Založí nový záznam (Id == 0) alebo aktualizuje existujúci. Vracia Id záznamu.
	/// </summary>
	Task<Dto<int>> UpdateRecordAsync(RecordDto recordDto, CancellationToken cancellationToken = default);

	/// <summary>
	/// Zmaže záznam.
	/// </summary>
	Task DeleteRecordAsync(Dto<int> id, CancellationToken cancellationToken = default);

	/// <summary>
	/// Vráti termíny, ktoré sa blížia alebo sú už po dátume, zoradené od najnaliehavejšieho.
	/// </summary>
	Task<List<ReminderDto>> GetUpcomingRemindersAsync(CancellationToken cancellationToken = default);
}
