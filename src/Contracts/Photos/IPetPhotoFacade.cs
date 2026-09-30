using Havit.ComponentModel;

namespace DogAppBlazor.Contracts.Photos;

[ApiContract]
public interface IPetPhotoFacade
{
	/// <summary>
	/// Uloží fotku do úložiska a vráti názov súboru. K entite (napr. psovi) sa fotka priradí
	/// až uložením entity s týmto názvom (napr. v <see cref="Dogs.DogDto.PhotoFileName"/>).
	/// </summary>
	Task<Dto<string>> UploadPhotoAsync(PetPhotoUploadDto photoUploadDto, CancellationToken cancellationToken = default);
}
