using System.Text.RegularExpressions;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DogAppBlazor.Contracts.Photos;
using Havit.Extensions.DependencyInjection.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DogAppBlazor.Facades.Photos;

/// <summary>
/// Univerzálne úložisko fotiek v Azure Blob Storage (kontajner <see cref="ContainerName"/>).
/// Kontajner je súkromný - fotky vydáva server.
/// </summary>
[Service(ServiceType = typeof(PetPhotoStorage), Lifetime = ServiceLifetime.Singleton)]
public partial class PetPhotoStorage(BlobServiceClient blobServiceClient)
{
	public const string ContainerName = "pet-photos";

	private readonly BlobContainerClient _containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);

	/// <summary>
	/// Uloží fotku pod novým jedinečným názvom a vráti ho.
	/// Názov sa nikdy nemení, preto sa fotka môže v prehliadači cachovať natrvalo.
	/// </summary>
	public async Task<string> UploadAsync(byte[] content, CancellationToken cancellationToken = default)
	{
		string fileName = $"{Guid.NewGuid():N}.jpg";
		await _containerClient.GetBlobClient(fileName).UploadAsync(
			new BinaryData(content),
			new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = PetPhotoUploadDto.ContentType } },
			cancellationToken);
		return fileName;
	}

	/// <summary>
	/// Otvorí fotku na čítanie. Ak neexistuje alebo názov nie je platný, vráti null.
	/// </summary>
	public async Task<Stream> OpenReadAsync(string fileName, CancellationToken cancellationToken = default)
	{
		if (!IsValidFileName(fileName))
		{
			return null;
		}

		try
		{
			return await _containerClient.GetBlobClient(fileName).OpenReadAsync(cancellationToken: cancellationToken);
		}
		catch (RequestFailedException ex) when (ex.Status == 404)
		{
			return null;
		}
	}

	public async Task DeleteAsync(string fileName, CancellationToken cancellationToken = default)
	{
		if (IsValidFileName(fileName))
		{
			await _containerClient.DeleteBlobIfExistsAsync(fileName, cancellationToken: cancellationToken);
		}
	}

	/// <summary>
	/// Názov súboru prichádza z URL aj od klienta - pripúšťame len názvy, ktoré sme sami vygenerovali.
	/// </summary>
	public static bool IsValidFileName(string fileName)
	{
		return (fileName is not null) && FileNameRegex().IsMatch(fileName);
	}

	[GeneratedRegex("^[0-9a-f]{32}\\.jpg$")]
	private static partial Regex FileNameRegex();
}
