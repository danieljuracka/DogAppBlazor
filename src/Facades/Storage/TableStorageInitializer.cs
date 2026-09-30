using Azure.Data.Tables;
using Havit.Extensions.DependencyInjection.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DogAppBlazor.Facades.Storage;

/// <summary>
/// Pri štarte aplikácie vytvorí v Storage accounte tabuľky, ktoré ešte neexistujú.
/// </summary>
[Service(ServiceType = typeof(TableStorageInitializer), Lifetime = ServiceLifetime.Singleton)]
public class TableStorageInitializer(TableServiceClient tableServiceClient)
{
	private readonly TableServiceClient _tableServiceClient = tableServiceClient;

	public async Task InitializeAsync(CancellationToken cancellationToken = default)
	{
		foreach (string tableName in TableNames.All)
		{
			await _tableServiceClient.CreateTableIfNotExistsAsync(tableName, cancellationToken);
		}
	}
}
