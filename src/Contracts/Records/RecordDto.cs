using System.ComponentModel.DataAnnotations;

namespace DogAppBlazor.Contracts.Records;

/// <summary>
/// Záznam o udalosti - návšteva veterinára, očkovanie, príznak, váženie a podobne.
/// </summary>
public class RecordDto : BaseDto
{
	[Range(1, int.MaxValue, ErrorMessage = "Vyber psa.")]
	public int DogId { get; set; }

	public RecordTypeEnum Type { get; set; } = RecordTypeEnum.VetVisit;

	[Required(ErrorMessage = "Dátum je povinný.")]
	public DateTime? OccurredOn { get; set; }

	[Required(ErrorMessage = "Popis je povinný.")]
	[MaxLength(100)]
	public string Title { get; set; }

	[MaxLength(2000)]
	public string Description { get; set; }

	[MaxLength(100)]
	public string VetName { get; set; }

	[Range(0, 100000, ErrorMessage = "Cena musí byť kladná.")]
	public decimal? Cost { get; set; }

	[Range(0, 200, ErrorMessage = "Hmotnosť musí byť kladná.")]
	public decimal? WeightKg { get; set; }

	/// <summary>
	/// Kedy sa má udalosť zopakovať - napríklad ďalšie očkovanie alebo odčervenie.
	/// </summary>
	public DateTime? NextDueOn { get; set; }

	/// <summary>
	/// Záznam, ktorého „ďalší termín“ tento záznam vybavuje. Kým takýto záznam existuje,
	/// pôvodný termín sa už nepripomína.
	/// </summary>
	public int? FollowUpOfRecordId { get; set; }
}
