using DogAppBlazor.Contracts.Dogs;
using Havit.Extensions.DependencyInjection.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DogAppBlazor.Facades.Dogs;

/// <summary>
/// Docasne uloziste psov v pamati. Nahradi sa napojenim na databazu.
/// Data zijú len pocas behu aplikacie.
/// </summary>
[Service(ServiceType = typeof(DogStorage), Lifetime = ServiceLifetime.Singleton)]
public class DogStorage
{
	private readonly Lock _lock = new();
	private readonly List<DogDto> _dogs;
	private int _nextId;

	public DogStorage()
	{
		_dogs =
		[
			new DogDto
			{
				Id = 1,
				Name = "Gusto",
				Color = "#4e9086",
				Breed = "Zlatý retríver",
				Sex = SexEnum.Male,
				BirthDate = new DateTime(2019, 5, 14),
				MicrochipNumber = "203098100123456",
				Note = "Miluje vodu a aport. Alergia na kuracie mäso."
			},
			new DogDto
			{
				Id = 2,
				Name = "Jonatán",
				Color = "#c48a1a",
				Breed = "Border kólia",
				Sex = SexEnum.Male,
				BirthDate = new DateTime(2022, 9, 2),
				MicrochipNumber = "203098100654321",
				Note = "Veľmi energický, potrebuje veľa pohybu."
			}
		];
		_nextId = 3;
	}

	public List<DogDto> GetAll()
	{
		lock (_lock)
		{
			return _dogs.ConvertAll(Clone);
		}
	}

	public DogDto Find(int id)
	{
		lock (_lock)
		{
			DogDto dog = _dogs.SingleOrDefault(d => d.Id == id);
			return (dog is null) ? null : Clone(dog);
		}
	}

	public int Upsert(DogDto dogDto)
	{
		lock (_lock)
		{
			if (dogDto.Id == 0)
			{
				DogDto newDog = Clone(dogDto);
				newDog.Id = _nextId++;
				_dogs.Add(newDog);
				return newDog.Id;
			}

			DogDto existing = _dogs.SingleOrDefault(d => d.Id == dogDto.Id)
				?? throw new InvalidOperationException($"Pes s Id {dogDto.Id} neexistuje.");

			existing.Name = dogDto.Name;
			existing.Breed = dogDto.Breed;
			existing.Sex = dogDto.Sex;
			existing.BirthDate = dogDto.BirthDate;
			existing.MicrochipNumber = dogDto.MicrochipNumber;
			existing.Note = dogDto.Note;
			existing.PhotoFileName = dogDto.PhotoFileName;
			existing.Color = dogDto.Color;

			return existing.Id;
		}
	}

	private static DogDto Clone(DogDto source)
	{
		return new DogDto
		{
			Id = source.Id,
			Name = source.Name,
			Breed = source.Breed,
			Sex = source.Sex,
			BirthDate = source.BirthDate,
			MicrochipNumber = source.MicrochipNumber,
			Note = source.Note,
			PhotoFileName = source.PhotoFileName,
			Color = source.Color
		};
	}
}
