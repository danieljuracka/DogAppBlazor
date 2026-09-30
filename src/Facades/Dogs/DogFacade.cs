using System.Text.RegularExpressions;
using DogAppBlazor.Contracts;
using DogAppBlazor.Contracts.Dogs;
using DogAppBlazor.Facades.Photos;
using Havit.Extensions.DependencyInjection.Abstractions;

namespace DogAppBlazor.Facades.Dogs;

[Service]
public class DogFacade(DogStorage dogStorage, PetPhotoStorage petPhotoStorage) : IDogFacade
{
	private readonly DogStorage _dogStorage = dogStorage;
	private readonly PetPhotoStorage _petPhotoStorage = petPhotoStorage;

	public async Task<List<DogListItemDto>> GetDogsAsync(CancellationToken cancellationToken = default)
	{
		List<DogListItemDto> result = (await _dogStorage.GetAllAsync(cancellationToken))
			.OrderBy(d => d.Name)
			.Select(d => new DogListItemDto
			{
				Id = d.Id,
				Name = d.Name,
				Breed = d.Breed,
				Sex = d.Sex,
				BirthDate = d.BirthDate,
				PhotoFileName = d.PhotoFileName,
				Color = d.Color
			})
			.ToList();

		return result;
	}

	public async Task<DogDto> GetDogAsync(Dto<int> id, CancellationToken cancellationToken = default)
	{
		Contract.Requires<ArgumentNullException>(id is not null);

		return await _dogStorage.FindAsync(id.Value, cancellationToken);
	}

	public async Task<Dto<int>> UpdateDogAsync(DogDto dogDto, CancellationToken cancellationToken = default)
	{
		Contract.Requires<ArgumentNullException>(dogDto is not null);
		Contract.Requires<ArgumentException>(!String.IsNullOrWhiteSpace(dogDto.Name));
		Contract.Requires<ArgumentException>((dogDto.Color is null) || Regex.IsMatch(dogDto.Color, DogDto.ColorPattern));
		Contract.Requires<ArgumentException>((dogDto.PhotoFileName is null) || PetPhotoStorage.IsValidFileName(dogDto.PhotoFileName));

		string previousPhotoFileName = (dogDto.Id == 0)
			? null
			: (await _dogStorage.FindAsync(dogDto.Id, cancellationToken))?.PhotoFileName;

		int id = await _dogStorage.UpsertAsync(dogDto, cancellationToken);

		// Pôvodnú fotku zmažeme až po úspešnom uložení profilu, aby pes nezostal s odkazom na neexistujúci súbor.
		if ((previousPhotoFileName is not null) && (previousPhotoFileName != dogDto.PhotoFileName))
		{
			await _petPhotoStorage.DeleteAsync(previousPhotoFileName, cancellationToken);
		}

		return Dto.FromValue(id);
	}
}
