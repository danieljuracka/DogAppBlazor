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

	/// <summary>
	/// Koľko najdrahších záznamov sa ukazuje v prehľade nákladov.
	/// </summary>
	private const int TopCostRecordsCount = 5;

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

	public async Task<CostSummaryDto> GetCostSummaryAsync(CostSummaryFilterDto filter, CancellationToken cancellationToken = default)
	{
		Contract.Requires<ArgumentNullException>(filter is not null);

		DateTime today = DateTime.Today;
		Dictionary<int, DogDto> dogs = (await _dogStorage.GetAllAsync(cancellationToken)).ToDictionary(d => d.Id);
		List<RecordDto> costRecords = (await _recordStorage.GetAllAsync(cancellationToken))
			.Where(r => r.Cost > 0)
			.Where(r => r.OccurredOn is not null)
			.ToList();

		List<int> availableYears = costRecords
			.Select(r => r.OccurredOn.Value.Year)
			.Append(today.Year)
			.Distinct()
			.OrderDescending()
			.ToList();

		List<RecordDto> records = costRecords
			.Where(r => (filter.DogId is null) || (r.DogId == filter.DogId.Value))
			.Where(r => (filter.Year is null) || (r.OccurredOn.Value.Year == filter.Year.Value))
			.ToList();

		decimal total = records.Sum(r => r.Cost.Value);

		return new CostSummaryDto
		{
			Total = total,
			RecordCount = records.Count,
			MonthlyAverage = Math.Round(total / GetMonthCount(records, filter.Year, today), 2),
			AvailableYears = availableYears,
			ByPeriod = GetCostsByPeriod(records, filter.Year, today),
			ByType = records
				.GroupBy(r => r.Type)
				.Select(g => new CostByTypeDto
				{
					Type = g.Key,
					Total = g.Sum(r => r.Cost.Value),
					RecordCount = g.Count()
				})
				.OrderByDescending(item => item.Total)
				.ToList(),
			ByDog = records
				.GroupBy(r => r.DogId)
				.Select(g => new CostByDogDto
				{
					DogId = g.Key,
					DogName = dogs.GetValueOrDefault(g.Key)?.Name,
					DogColor = dogs.GetValueOrDefault(g.Key)?.Color,
					Total = g.Sum(r => r.Cost.Value),
					RecordCount = g.Count()
				})
				.OrderByDescending(item => item.Total)
				.ToList(),
			TopRecords = records
				.OrderByDescending(r => r.Cost.Value)
				.ThenByDescending(r => r.OccurredOn)
				.Take(TopCostRecordsCount)
				.Select(r => new CostRecordDto
				{
					RecordId = r.Id,
					DogId = r.DogId,
					DogName = dogs.GetValueOrDefault(r.DogId)?.Name,
					Type = r.Type,
					Title = r.Title,
					OccurredOn = r.OccurredOn.Value,
					Cost = r.Cost.Value
				})
				.ToList()
		};
	}

	/// <summary>
	/// Počet mesiacov, na ktoré sa rozpočítava priemer. Pri aktuálnom roku len uplynulé mesiace,
	/// pri celom období od mesiaca prvého nákladu po dnešok.
	/// </summary>
	private static int GetMonthCount(List<RecordDto> records, int? year, DateTime today)
	{
		if (year is not null)
		{
			return (year.Value == today.Year) ? today.Month : 12;
		}

		if (records.Count == 0)
		{
			return 1;
		}

		DateTime first = records.Min(r => r.OccurredOn.Value);
		return Math.Max(1, ((today.Year - first.Year) * 12) + today.Month - first.Month + 1);
	}

	/// <summary>
	/// Pri zvolenom roku všetkých 12 mesiacov, pri celom období každý rok od prvého nákladu - aj tie bez nákladov,
	/// aby graf nemal diery.
	/// </summary>
	private static List<CostByPeriodDto> GetCostsByPeriod(List<RecordDto> records, int? year, DateTime today)
	{
		if (year is not null)
		{
			return Enumerable.Range(1, 12)
				.Select(month => new CostByPeriodDto
				{
					Year = year.Value,
					Month = month,
					Total = records.Where(r => r.OccurredOn.Value.Month == month).Sum(r => r.Cost.Value)
				})
				.ToList();
		}

		if (records.Count == 0)
		{
			return [];
		}

		int firstYear = records.Min(r => r.OccurredOn.Value.Year);
		int lastYear = Math.Max(today.Year, records.Max(r => r.OccurredOn.Value.Year));

		return Enumerable.Range(firstYear, lastYear - firstYear + 1)
			.Select(y => new CostByPeriodDto
			{
				Year = y,
				Total = records.Where(r => r.OccurredOn.Value.Year == y).Sum(r => r.Cost.Value)
			})
			.ToList();
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
