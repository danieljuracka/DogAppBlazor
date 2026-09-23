using System.Text.RegularExpressions;
using DogAppBlazor.Contracts;
using DogAppBlazor.Contracts.Dogs;
using Havit.Extensions.DependencyInjection.Abstractions;

namespace DogAppBlazor.Facades.Dogs;

[Service]
public class DogFacade(DogStorage dogStorage) : IDogFacade
{
	private readonly DogStorage _dogStorage = dogStorage;

	public Task<List<DogListItemDto>> GetDogsAsync(CancellationToken cancellationToken = default)
	{
		List<DogListItemDto> result = _dogStorage.GetAll()
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

		return Task.FromResult(result);
	}

	public Task<DogDto> GetDogAsync(Dto<int> id, CancellationToken cancellationToken = default)
	{
		Contract.Requires<ArgumentNullException>(id is not null);

		return Task.FromResult(_dogStorage.Find(id.Value));
	}

	public Task<Dto<int>> UpdateDogAsync(DogDto dogDto, CancellationToken cancellationToken = default)
	{
		Contract.Requires<ArgumentNullException>(dogDto is not null);
		Contract.Requires<ArgumentException>(!String.IsNullOrWhiteSpace(dogDto.Name));
		Contract.Requires<ArgumentException>((dogDto.Color is null) || Regex.IsMatch(dogDto.Color, DogDto.ColorPattern));

		return Task.FromResult(Dto.FromValue(_dogStorage.Upsert(dogDto)));
	}
}
