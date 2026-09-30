using Azure;
using Azure.Data.Tables;
using Havit.Extensions.DependencyInjection.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DogAppBlazor.Facades.Storage;

/// <summary>
/// Prideľuje rastúce číselné Id. Posledné pridelené Id každého počítadla je uložené
/// v tabuľke <see cref="TableNames.Counters"/>. Súbežné zápisy rieši optimistická
/// konkurencia cez ETag - pri kolízii sa pokus zopakuje.
/// </summary>
[Service(ServiceType = typeof(IdGenerator), Lifetime = ServiceLifetime.Singleton)]
public class IdGenerator(TableServiceClient tableServiceClient)
{
	private const string PartitionKey = "Counter";
	private const string LastIdProperty = "LastId";
	private const int MaxAttempts = 10;

	private readonly TableClient _tableClient = tableServiceClient.GetTableClient(TableNames.Counters);

	public async Task<int> GetNextIdAsync(string counterName, CancellationToken cancellationToken = default)
	{
		for (int attempt = 1; attempt <= MaxAttempts; attempt++)
		{
			NullableResponse<TableEntity> response = await _tableClient.GetEntityIfExistsAsync<TableEntity>(PartitionKey, counterName, cancellationToken: cancellationToken);

			try
			{
				if (!response.HasValue)
				{
					await _tableClient.AddEntityAsync(new TableEntity(PartitionKey, counterName) { [LastIdProperty] = 1 }, cancellationToken);
					return 1;
				}

				TableEntity counter = response.Value;
				int nextId = counter.GetInt32(LastIdProperty).GetValueOrDefault() + 1;
				counter[LastIdProperty] = nextId;
				await _tableClient.UpdateEntityAsync(counter, counter.ETag, TableUpdateMode.Replace, cancellationToken);
				return nextId;
			}
			catch (RequestFailedException ex) when ((ex.Status == 409) || (ex.Status == 412))
			{
				// Iný požiadavok medzitým počítadlo založil (409) alebo zmenil (412) - skúsime znova.
			}
		}

		throw new InvalidOperationException($"Nepodarilo sa prideliť Id pre '{counterName}'.");
	}
}
