using Azure;
using Azure.Data.Tables;
using DogAppBlazor.Contracts.Records;
using DogAppBlazor.Facades.Storage;
using Havit.Extensions.DependencyInjection.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DogAppBlazor.Facades.Records;

/// <summary>
/// Úložisko záznamov v Azure Table Storage (tabuľka <see cref="TableNames.Records"/>).
/// Všetky záznamy sú v jednej partícii, kľúčom riadku je Id.
/// </summary>
[Service(ServiceType = typeof(RecordStorage), Lifetime = ServiceLifetime.Singleton)]
public class RecordStorage(TableServiceClient tableServiceClient, IdGenerator idGenerator)
{
	private const string PartitionKey = "Record";

	private readonly TableClient _tableClient = tableServiceClient.GetTableClient(TableNames.Records);
	private readonly IdGenerator _idGenerator = idGenerator;

	public async Task<List<RecordDto>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		List<RecordDto> result = [];
		await foreach (TableEntity entity in _tableClient.QueryAsync<TableEntity>(e => e.PartitionKey == PartitionKey, cancellationToken: cancellationToken))
		{
			result.Add(ToDto(entity));
		}
		return result;
	}

	public async Task<RecordDto> FindAsync(int id, CancellationToken cancellationToken = default)
	{
		NullableResponse<TableEntity> response = await _tableClient.GetEntityIfExistsAsync<TableEntity>(PartitionKey, TableEntityExtensions.ToRowKey(id), cancellationToken: cancellationToken);
		return response.HasValue ? ToDto(response.Value) : null;
	}

	public async Task<int> UpsertAsync(RecordDto recordDto, CancellationToken cancellationToken = default)
	{
		if (recordDto.Id == 0)
		{
			int newId = await _idGenerator.GetNextIdAsync(TableNames.Records, cancellationToken);
			await _tableClient.AddEntityAsync(ToEntity(newId, recordDto), cancellationToken);
			return newId;
		}

		try
		{
			await _tableClient.UpdateEntityAsync(ToEntity(recordDto.Id, recordDto), ETag.All, TableUpdateMode.Replace, cancellationToken);
		}
		catch (RequestFailedException ex) when (ex.Status == 404)
		{
			throw new InvalidOperationException($"Záznam s Id {recordDto.Id} neexistuje.", ex);
		}

		return recordDto.Id;
	}

	public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
	{
		// Zmazanie neexistujúceho záznamu neskončí chybou.
		await _tableClient.DeleteEntityAsync(PartitionKey, TableEntityExtensions.ToRowKey(id), cancellationToken: cancellationToken);
	}

	private static TableEntity ToEntity(int id, RecordDto recordDto)
	{
		return new TableEntity(PartitionKey, TableEntityExtensions.ToRowKey(id))
		{
			[nameof(RecordDto.DogId)] = recordDto.DogId,
			[nameof(RecordDto.Type)] = recordDto.Type.ToString(),
			[nameof(RecordDto.OccurredOn)] = TableEntityExtensions.ToStorageDate(recordDto.OccurredOn),
			[nameof(RecordDto.Title)] = recordDto.Title,
			[nameof(RecordDto.Description)] = recordDto.Description,
			[nameof(RecordDto.VetName)] = recordDto.VetName,
			[nameof(RecordDto.Cost)] = TableEntityExtensions.ToStorageDecimal(recordDto.Cost),
			[nameof(RecordDto.WeightKg)] = TableEntityExtensions.ToStorageDecimal(recordDto.WeightKg),
			[nameof(RecordDto.NextDueOn)] = TableEntityExtensions.ToStorageDate(recordDto.NextDueOn),
			[nameof(RecordDto.FollowUpOfRecordId)] = recordDto.FollowUpOfRecordId
		};
	}

	private static RecordDto ToDto(TableEntity entity)
	{
		return new RecordDto
		{
			Id = entity.GetId(),
			DogId = entity.GetInt32(nameof(RecordDto.DogId)).GetValueOrDefault(),
			Type = entity.GetEnum<RecordTypeEnum>(nameof(RecordDto.Type)).GetValueOrDefault(RecordTypeEnum.Other),
			OccurredOn = entity.GetDate(nameof(RecordDto.OccurredOn)),
			Title = entity.GetString(nameof(RecordDto.Title)),
			Description = entity.GetString(nameof(RecordDto.Description)),
			VetName = entity.GetString(nameof(RecordDto.VetName)),
			Cost = entity.GetDecimal(nameof(RecordDto.Cost)),
			WeightKg = entity.GetDecimal(nameof(RecordDto.WeightKg)),
			NextDueOn = entity.GetDate(nameof(RecordDto.NextDueOn)),
			FollowUpOfRecordId = entity.GetInt32(nameof(RecordDto.FollowUpOfRecordId))
		};
	}
}
