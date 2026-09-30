using Azure.Data.Tables;
using Azure.Storage.Blobs;
using DogAppBlazor.Facades.Photos;
using Havit.Extensions.DependencyInjection.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DogAppBlazor.Facades.Storage;

/// <summary>
/// Pri štarte aplikácie vytvorí v Storage accounte tabuľky a blob kontajnery, ktoré ešte neexistujú.
/// </summary>
[Service(ServiceType = typeof(TableStorageInitializer), Lifetime = ServiceLifetime.Singleton)]
public class TableStorageInitializer(TableServiceClient tableServiceClient, BlobServiceClient blobServiceClient)
{
	private readonly TableServiceClient _tableServiceClient = tableServiceClient;
	private readonly BlobServiceClient _blobServiceClient = blobServiceClient;

	public async Task InitializeAsync(CancellationToken cancellationToken = default)
	{
		foreach (string tableName in TableNames.All)
		{
			await _tableServiceClient.CreateTableIfNotExistsAsync(tableName, cancellationToken);
		}

		// Kontajner je súkromný (bez verejného prístupu) - fotky vydáva server.
		await _blobServiceClient.GetBlobContainerClient(PetPhotoStorage.ContainerName).CreateIfNotExistsAsync(cancellationToken: cancellationToken);
	}
}
