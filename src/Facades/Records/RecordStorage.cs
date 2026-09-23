using DogAppBlazor.Contracts.Records;
using Havit.Extensions.DependencyInjection.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DogAppBlazor.Facades.Records;

/// <summary>
/// Dočasné úložisko záznamov v pamäti. Nahradí sa napojením na databázu.
/// Dáta žijú len počas behu aplikácie.
/// </summary>
[Service(ServiceType = typeof(RecordStorage), Lifetime = ServiceLifetime.Singleton)]
public class RecordStorage
{
	private readonly Lock _lock = new();
	private readonly List<RecordDto> _records;
	private int _nextId;

	public RecordStorage()
	{
		// Ukážkové dáta sú viazané na dnešok, aby termíny dávali zmysel aj o mesiac.
		DateTime today = DateTime.Today;

		_records =
		[
			new RecordDto
			{
				Id = 1,
				DogId = 1,
				Type = RecordTypeEnum.Vaccination,
				OccurredOn = today.AddMonths(-6),
				Title = "Očkovanie proti besnote",
				VetName = "MVDr. Kováčová",
				Cost = 35m,
				NextDueOn = today.AddMonths(6)
			},
			new RecordDto
			{
				Id = 2,
				DogId = 1,
				Type = RecordTypeEnum.Deworming,
				OccurredOn = today.AddDays(-110),
				Title = "Odčervenie",
				NextDueOn = today.AddDays(-20)
			},
			new RecordDto
			{
				Id = 3,
				DogId = 1,
				Type = RecordTypeEnum.Weight,
				OccurredOn = today.AddDays(-8),
				Title = "Váženie",
				WeightKg = 32.4m
			},
			new RecordDto
			{
				Id = 4,
				DogId = 2,
				Type = RecordTypeEnum.Symptom,
				OccurredOn = today.AddDays(-16),
				Title = "Hnačka",
				Description = "Dva dni riedka stolica, inak čulý a je normálne. Tretí deň to prešlo."
			},
			new RecordDto
			{
				Id = 5,
				DogId = 2,
				Type = RecordTypeEnum.VetVisit,
				OccurredOn = today.AddDays(-14),
				Title = "Kontrola po hnačke",
				Description = "Bez nálezu, odporúčaná ľahká diéta na týždeň.",
				VetName = "MVDr. Kováčová",
				Cost = 28.50m
			},
			new RecordDto
			{
				Id = 6,
				DogId = 2,
				Type = RecordTypeEnum.Vaccination,
				OccurredOn = today.AddMonths(-11),
				Title = "Kombinovaná vakcína",
				VetName = "MVDr. Kováčová",
				Cost = 42m,
				NextDueOn = today.AddDays(12)
			}
		];

		_nextId = 7;
	}

	public List<RecordDto> GetAll()
	{
		lock (_lock)
		{
			return _records.ConvertAll(Clone);
		}
	}

	public RecordDto Find(int id)
	{
		lock (_lock)
		{
			RecordDto record = _records.SingleOrDefault(r => r.Id == id);
			return (record is null) ? null : Clone(record);
		}
	}

	public int Upsert(RecordDto recordDto)
	{
		lock (_lock)
		{
			if (recordDto.Id == 0)
			{
				RecordDto newRecord = Clone(recordDto);
				newRecord.Id = _nextId++;
				_records.Add(newRecord);
				return newRecord.Id;
			}

			RecordDto existing = _records.SingleOrDefault(r => r.Id == recordDto.Id)
				?? throw new InvalidOperationException($"Záznam s Id {recordDto.Id} neexistuje.");

			existing.DogId = recordDto.DogId;
			existing.Type = recordDto.Type;
			existing.OccurredOn = recordDto.OccurredOn;
			existing.Title = recordDto.Title;
			existing.Description = recordDto.Description;
			existing.VetName = recordDto.VetName;
			existing.Cost = recordDto.Cost;
			existing.WeightKg = recordDto.WeightKg;
			existing.NextDueOn = recordDto.NextDueOn;
			existing.FollowUpOfRecordId = recordDto.FollowUpOfRecordId;

			return existing.Id;
		}
	}

	public void Delete(int id)
	{
		lock (_lock)
		{
			_records.RemoveAll(r => r.Id == id);
		}
	}

	private static RecordDto Clone(RecordDto source)
	{
		return new RecordDto
		{
			Id = source.Id,
			DogId = source.DogId,
			Type = source.Type,
			OccurredOn = source.OccurredOn,
			Title = source.Title,
			Description = source.Description,
			VetName = source.VetName,
			Cost = source.Cost,
			WeightKg = source.WeightKg,
			NextDueOn = source.NextDueOn,
			FollowUpOfRecordId = source.FollowUpOfRecordId
		};
	}
}
