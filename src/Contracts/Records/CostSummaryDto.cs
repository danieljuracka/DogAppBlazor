namespace DogAppBlazor.Contracts.Records;

/// <summary>
/// Súhrn nákladov zo záznamov, ktoré majú vyplnenú cenu.
/// </summary>
public class CostSummaryDto
{
	public decimal Total { get; set; }

	/// <summary>
	/// Počet záznamov s cenou, ktoré sa do súhrnu započítali.
	/// </summary>
	public int RecordCount { get; set; }

	/// <summary>
	/// Priemerný náklad na mesiac - za uplynulé mesiace zvoleného roka,
	/// pri celom období od prvého záznamu s cenou po dnešok.
	/// </summary>
	public decimal MonthlyAverage { get; set; }

	/// <summary>
	/// Roky, v ktorých existuje aspoň jeden náklad (bez ohľadu na psa), vrátane aktuálneho roka. Zoradené od najnovšieho.
	/// </summary>
	public List<int> AvailableYears { get; set; } = [];

	/// <summary>
	/// Pri zvolenom roku všetkých 12 mesiacov, pri celom období jednotlivé roky. Zoradené chronologicky.
	/// </summary>
	public List<CostByPeriodDto> ByPeriod { get; set; } = [];

	/// <summary>
	/// Zoradené od najdrahšieho druhu.
	/// </summary>
	public List<CostByTypeDto> ByType { get; set; } = [];

	/// <summary>
	/// Zoradené od psa s najvyššími nákladmi.
	/// </summary>
	public List<CostByDogDto> ByDog { get; set; } = [];

	/// <summary>
	/// Najdrahšie jednotlivé záznamy.
	/// </summary>
	public List<CostRecordDto> TopRecords { get; set; } = [];
}

public class CostByPeriodDto
{
	public int Year { get; set; }

	/// <summary>
	/// Mesiac 1-12. Null, ak položka predstavuje celý rok.
	/// </summary>
	public int? Month { get; set; }

	public decimal Total { get; set; }
}

public class CostByTypeDto
{
	public RecordTypeEnum Type { get; set; }

	public decimal Total { get; set; }

	public int RecordCount { get; set; }
}

public class CostByDogDto
{
	public int DogId { get; set; }

	public string DogName { get; set; }

	public string DogColor { get; set; }

	public decimal Total { get; set; }

	public int RecordCount { get; set; }
}

public class CostRecordDto
{
	public int RecordId { get; set; }

	public int DogId { get; set; }

	public string DogName { get; set; }

	public RecordTypeEnum Type { get; set; }

	public string Title { get; set; }

	public DateTime OccurredOn { get; set; }

	public decimal Cost { get; set; }
}
