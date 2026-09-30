using DogAppBlazor.Contracts;
using DogAppBlazor.Contracts.Dogs;
using DogAppBlazor.Contracts.Records;
using DogAppBlazor.Facades.Dogs;
using Havit.Extensions.DependencyInjection.Abstractions;

namespace DogAppBlazor.Facades.Records;

[Service]
public class RecordFacade(RecordStorage recordStorage, DogStorage dogStorage) : IRecordFacade
{
	/// <summary>
	/// Koľko dní dopredu sa pripomínajú blížiace sa termíny.
	/// </summary>
	private const int ReminderHorizonDays = 30;

	private readonly RecordStorage _recordStorage = recordStorage;
	private readonly DogStorage _dogStorage = dogStorage;

	public async Task<List<RecordListItemDto>> GetRecordsAsync(RecordFilterDto filter, CancellationToken cancellationToken = default)
	{
		Contract.Requires<ArgumentNullException>(filter is not null);

		Dictionary<int, string> dogNames = await GetDogNamesAsync(cancellationToken);
		List<RecordDto> allRecords = await _recordStorage.GetAllAsync(cancellationToken);
		HashSet<int> completedIds = GetCompletedRecordIds(allRecords);

		List<RecordListItemDto> result = allRecords
			.Where(r => (filter.DogId is null) || (r.DogId == filter.DogId.Value))
			.Where(r => (filter.Type is null) || (r.Type == filter.Type.Value))
			.OrderByDescending(r => r.OccurredOn)
			.ThenByDescending(r => r.Id)
			.Select(r => new RecordListItemDto
			{
				Id = r.Id,
				DogId = r.DogId,
				DogName = dogNames.GetValueOrDefault(r.DogId),
				Type = r.Type,
				OccurredOn = r.OccurredOn.GetValueOrDefault(),
				Title = r.Title,
				Description = r.Description,
				VetName = r.VetName,
				Cost = r.Cost,
				WeightKg = r.WeightKg,
				NextDueOn = r.NextDueOn,
				FollowUpOfRecordId = r.FollowUpOfRecordId,
				IsNextDueCompleted = completedIds.Contains(r.Id)
			})
			.ToList();

		return result;
	}

	public async Task<RecordDto> GetRecordAsync(Dto<int> id, CancellationToken cancellationToken = default)
	{
		Contract.Requires<ArgumentNullException>(id is not null);

		return await _recordStorage.FindAsync(id.Value, cancellationToken);
	}

	public async Task<Dto<int>> UpdateRecordAsync(RecordDto recordDto, CancellationToken cancellationToken = default)
	{
		Contract.Requires<ArgumentNullException>(recordDto is not null);
		Contract.Requires<ArgumentException>(recordDto.DogId > 0);
		Contract.Requires<ArgumentException>(recordDto.OccurredOn is not null);
		Contract.Requires<ArgumentException>(!String.IsNullOrWhiteSpace(recordDto.Title));

		return Dto.FromValue(await _recordStorage.UpsertAsync(recordDto, cancellationToken));
	}

	public async Task DeleteRecordAsync(Dto<int> id, CancellationToken cancellationToken = default)
	{
		Contract.Requires<ArgumentNullException>(id is not null);

		await _recordStorage.DeleteAsync(id.Value, cancellationToken);
	}

	public async Task<List<ReminderDto>> GetUpcomingRemindersAsync(CancellationToken cancellationToken = default)
	{
		DateTime today = DateTime.Today;
		DateTime horizon = today.AddDays(ReminderHorizonDays);
		Dictionary<int, string> dogNames = await GetDogNamesAsync(cancellationToken);
		List<RecordDto> allRecords = await _recordStorage.GetAllAsync(cancellationToken);
		HashSet<int> completedIds = GetCompletedRecordIds(allRecords);

		List<ReminderDto> result = allRecords
			.Where(r => r.NextDueOn is not null)
			.Where(r => !completedIds.Contains(r.Id))
			.Where(r => r.NextDueOn.Value.Date <= horizon)
			.OrderBy(r => r.NextDueOn.Value)
			.Select(r => new ReminderDto
			{
				RecordId = r.Id,
				DogId = r.DogId,
				DogName = dogNames.GetValueOrDefault(r.DogId),
				Type = r.Type,
				Title = r.Title,
				DueOn = r.NextDueOn.Value.Date,
				IsOverdue = r.NextDueOn.Value.Date < today
			})
			.ToList();

		return result;
	}

	/// <summary>
	/// Id záznamov, ktorých „ďalší termín“ už vybavil iný záznam.
	/// Keď sa nadväzujúci záznam zmaže, termín sa automaticky znova otvorí.
	/// </summary>
	private static HashSet<int> GetCompletedRecordIds(IEnumerable<RecordDto> records)
	{
		return records
			.Where(r => r.FollowUpOfRecordId is not null)
			.Select(r => r.FollowUpOfRecordId.Value)
			.ToHashSet();
	}

	private async Task<Dictionary<int, string>> GetDogNamesAsync(CancellationToken cancellationToken)
	{
		return (await _dogStorage.GetAllAsync(cancellationToken)).ToDictionary(d => d.Id, d => d.Name);
	}
}
