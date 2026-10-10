using Azure;
using Azure.Data.Tables;
using DogAppBlazor.Contracts.PackingList;
using DogAppBlazor.Facades.Storage;
using Havit.Extensions.DependencyInjection.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DogAppBlazor.Facades.PackingList;

/// <summary>
/// Úložisko položiek baliaceho zoznamu v Azure Table Storage (tabuľka <see cref="TableNames.PackingItems"/>).
/// Všetky položky sú v jednej partícii, kľúčom riadku je Id. V samostatnej partícii je značka,
/// že predvolené položky už boli vložené - aby sa po zmazaní všetkých položiek nevrátili.
/// </summary>
[Service(ServiceType = typeof(PackingItemStorage), Lifetime = ServiceLifetime.Singleton)]
public class PackingItemStorage(TableServiceClient tableServiceClient, IdGenerator idGenerator)
{
	private const string PartitionKey = "Item";
	private const string MetaPartitionKey = "Meta";
	private const string DefaultsSeededRowKey = "DefaultsSeeded";

	/// <summary>
	/// Maximálny počet operácií v jednej transakcii Table Storage.
	/// </summary>
	private const int MaxTransactionSize = 100;

	private readonly TableClient _tableClient = tableServiceClient.GetTableClient(TableNames.PackingItems);
	private readonly IdGenerator _idGenerator = idGenerator;

	public async Task<List<PackingItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		List<PackingItemDto> result = await QueryAllAsync(cancellationToken);
		if ((result.Count == 0) && await TrySeedDefaultsAsync(cancellationToken))
		{
			result = await QueryAllAsync(cancellationToken);
		}
		return result;
	}

	public async Task<int> UpsertAsync(PackingItemDto packingItemDto, CancellationToken cancellationToken = default)
	{
		if (packingItemDto.Id == 0)
		{
			int newId = await _idGenerator.GetNextIdAsync(TableNames.PackingItems, cancellationToken);
			await _tableClient.AddEntityAsync(ToEntity(newId, packingItemDto), cancellationToken);
			return newId;
		}

		try
		{
			await _tableClient.UpdateEntityAsync(ToEntity(packingItemDto.Id, packingItemDto), ETag.All, TableUpdateMode.Replace, cancellationToken);
		}
		catch (RequestFailedException ex) when (ex.Status == 404)
		{
			throw new InvalidOperationException($"Položka s Id {packingItemDto.Id} neexistuje.", ex);
		}

		return packingItemDto.Id;
	}

	public async Task SetPackedAsync(int id, bool isPacked, CancellationToken cancellationToken = default)
	{
		try
		{
			await _tableClient.UpdateEntityAsync(ToPackedEntity(id, isPacked), ETag.All, TableUpdateMode.Merge, cancellationToken);
		}
		catch (RequestFailedException ex) when (ex.Status == 404)
		{
			throw new InvalidOperationException($"Položka s Id {id} neexistuje.", ex);
		}
	}

	/// <summary>
	/// Nastaví stav zbalenia všetkým položkám. Zapisujú sa len položky, ktorým sa stav mení.
	/// </summary>
	public async Task SetAllPackedAsync(bool isPacked, CancellationToken cancellationToken = default)
	{
		List<TableTransactionAction> actions = (await QueryAllAsync(cancellationToken))
			.Where(item => item.IsPacked != isPacked)
			.Select(item => new TableTransactionAction(TableTransactionActionType.UpdateMerge, ToPackedEntity(item.Id, isPacked)))
			.ToList();

		await SubmitInTransactionsAsync(actions, cancellationToken);
	}

	public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
	{
		// Zmazanie neexistujúcej položky neskončí chybou.
		await _tableClient.DeleteEntityAsync(PartitionKey, TableEntityExtensions.ToRowKey(id), cancellationToken: cancellationToken);
	}

	private async Task<List<PackingItemDto>> QueryAllAsync(CancellationToken cancellationToken)
	{
		List<PackingItemDto> result = [];
		await foreach (TableEntity entity in _tableClient.QueryAsync<TableEntity>(e => e.PartitionKey == PartitionKey, cancellationToken: cancellationToken))
		{
			result.Add(ToDto(entity));
		}
		return result;
	}

	/// <summary>
	/// Vloží predvolené položky, ak ešte neboli vložené. Značka sa zapisuje ako prvá, takže
	/// pri súbežných požiadavkách vloží položky len jedna z nich (ostatné dostanú 409).
	/// </summary>
	private async Task<bool> TrySeedDefaultsAsync(CancellationToken cancellationToken)
	{
		try
		{
			await _tableClient.AddEntityAsync(new TableEntity(MetaPartitionKey, DefaultsSeededRowKey), cancellationToken);
		}
		catch (RequestFailedException ex) when (ex.Status == 409)
		{
			return false;
		}

		IReadOnlyList<(string Category, string Name)> defaults = PackingListDefaults.Items;
		int firstId = await _idGenerator.GetNextIdsAsync(TableNames.PackingItems, defaults.Count, cancellationToken);

		List<TableTransactionAction> actions = defaults
			.Select((item, index) => new TableTransactionAction(TableTransactionActionType.Add, ToEntity(firstId + index, new PackingItemDto
			{
				Category = item.Category,
				Name = item.Name
			})))
			.ToList();

		await SubmitInTransactionsAsync(actions, cancellationToken);
		return true;
	}

	private async Task SubmitInTransactionsAsync(List<TableTransactionAction> actions, CancellationToken cancellationToken)
	{
		foreach (TableTransactionAction[] chunk in actions.Chunk(MaxTransactionSize))
		{
			await _tableClient.SubmitTransactionAsync(chunk, cancellationToken);
		}
	}

	private static TableEntity ToPackedEntity(int id, bool isPacked)
	{
		return new TableEntity(PartitionKey, TableEntityExtensions.ToRowKey(id))
		{
			[nameof(PackingItemDto.IsPacked)] = isPacked
		};
	}

	private static TableEntity ToEntity(int id, PackingItemDto packingItemDto)
	{
		return new TableEntity(PartitionKey, TableEntityExtensions.ToRowKey(id))
		{
			[nameof(PackingItemDto.Name)] = packingItemDto.Name,
			[nameof(PackingItemDto.Category)] = packingItemDto.Category,
			[nameof(PackingItemDto.Note)] = packingItemDto.Note,
			[nameof(PackingItemDto.IsPacked)] = packingItemDto.IsPacked
		};
	}

	private static PackingItemDto ToDto(TableEntity entity)
	{
		return new PackingItemDto
		{
			Id = entity.GetId(),
			Name = entity.GetString(nameof(PackingItemDto.Name)),
			Category = entity.GetString(nameof(PackingItemDto.Category)),
			Note = entity.GetString(nameof(PackingItemDto.Note)),
			IsPacked = entity.GetBoolean(nameof(PackingItemDto.IsPacked)).GetValueOrDefault()
		};
	}
}
