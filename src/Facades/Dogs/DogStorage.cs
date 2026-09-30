using Azure;
using Azure.Data.Tables;
using DogAppBlazor.Contracts.Dogs;
using DogAppBlazor.Facades.Storage;
using Havit.Extensions.DependencyInjection.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DogAppBlazor.Facades.Dogs;

/// <summary>
/// Úložisko psov v Azure Table Storage (tabuľka <see cref="TableNames.Dogs"/>).
/// Všetci psi sú v jednej partícii, kľúčom riadku je Id.
/// </summary>
[Service(ServiceType = typeof(DogStorage), Lifetime = ServiceLifetime.Singleton)]
public class DogStorage(TableServiceClient tableServiceClient, IdGenerator idGenerator)
{
	private const string PartitionKey = "Dog";

	private readonly TableClient _tableClient = tableServiceClient.GetTableClient(TableNames.Dogs);
	private readonly IdGenerator _idGenerator = idGenerator;

	public async Task<List<DogDto>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		List<DogDto> result = [];
		await foreach (TableEntity entity in _tableClient.QueryAsync<TableEntity>(e => e.PartitionKey == PartitionKey, cancellationToken: cancellationToken))
		{
			result.Add(ToDto(entity));
		}
		return result;
	}

	public async Task<DogDto> FindAsync(int id, CancellationToken cancellationToken = default)
	{
		NullableResponse<TableEntity> response = await _tableClient.GetEntityIfExistsAsync<TableEntity>(PartitionKey, TableEntityExtensions.ToRowKey(id), cancellationToken: cancellationToken);
		return response.HasValue ? ToDto(response.Value) : null;
	}

	public async Task<int> UpsertAsync(DogDto dogDto, CancellationToken cancellationToken = default)
	{
		if (dogDto.Id == 0)
		{
			int newId = await _idGenerator.GetNextIdAsync(TableNames.Dogs, cancellationToken);
			await _tableClient.AddEntityAsync(ToEntity(newId, dogDto), cancellationToken);
			return newId;
		}

		try
		{
			await _tableClient.UpdateEntityAsync(ToEntity(dogDto.Id, dogDto), ETag.All, TableUpdateMode.Replace, cancellationToken);
		}
		catch (RequestFailedException ex) when (ex.Status == 404)
		{
			throw new InvalidOperationException($"Pes s Id {dogDto.Id} neexistuje.", ex);
		}

		return dogDto.Id;
	}

	private static TableEntity ToEntity(int id, DogDto dogDto)
	{
		return new TableEntity(PartitionKey, TableEntityExtensions.ToRowKey(id))
		{
			[nameof(DogDto.Name)] = dogDto.Name,
			[nameof(DogDto.Breed)] = dogDto.Breed,
			[nameof(DogDto.Sex)] = dogDto.Sex?.ToString(),
			[nameof(DogDto.BirthDate)] = TableEntityExtensions.ToStorageDate(dogDto.BirthDate),
			[nameof(DogDto.MicrochipNumber)] = dogDto.MicrochipNumber,
			[nameof(DogDto.Note)] = dogDto.Note,
			[nameof(DogDto.PhotoFileName)] = dogDto.PhotoFileName,
			[nameof(DogDto.Color)] = dogDto.Color
		};
	}

	private static DogDto ToDto(TableEntity entity)
	{
		return new DogDto
		{
			Id = entity.GetId(),
			Name = entity.GetString(nameof(DogDto.Name)),
			Breed = entity.GetString(nameof(DogDto.Breed)),
			Sex = entity.GetEnum<SexEnum>(nameof(DogDto.Sex)),
			BirthDate = entity.GetDate(nameof(DogDto.BirthDate)),
			MicrochipNumber = entity.GetString(nameof(DogDto.MicrochipNumber)),
			Note = entity.GetString(nameof(DogDto.Note)),
			PhotoFileName = entity.GetString(nameof(DogDto.PhotoFileName)),
			Color = entity.GetString(nameof(DogDto.Color))
		};
	}
}
