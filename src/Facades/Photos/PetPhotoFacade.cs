using DogAppBlazor.Contracts;
using DogAppBlazor.Contracts.Photos;
using Havit.Extensions.DependencyInjection.Abstractions;

namespace DogAppBlazor.Facades.Photos;

[Service]
public class PetPhotoFacade(PetPhotoStorage petPhotoStorage) : IPetPhotoFacade
{
	/// <summary>
	/// Začiatok každého JPEG súboru (SOI marker).
	/// </summary>
	private static readonly byte[] JpegSignature = [0xFF, 0xD8, 0xFF];

	private readonly PetPhotoStorage _petPhotoStorage = petPhotoStorage;

	public async Task<Dto<string>> UploadPhotoAsync(PetPhotoUploadDto photoUploadDto, CancellationToken cancellationToken = default)
	{
		Contract.Requires<ArgumentNullException>(photoUploadDto?.Content is not null);
		Contract.Requires<ArgumentException>(photoUploadDto.Content.Length <= PetPhotoUploadDto.MaxSize, "Fotka je príliš veľká.");
		Contract.Requires<ArgumentException>(photoUploadDto.Content.AsSpan().StartsWith(JpegSignature), "Fotka musí byť vo formáte JPEG.");

		return Dto.FromValue(await _petPhotoStorage.UploadAsync(photoUploadDto.Content, cancellationToken));
	}
}
