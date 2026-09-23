namespace DogAppBlazor.Contracts.Records;

/// <summary>
/// Položka zoznamu záznamov. Obsahuje aj meno psa, aby sa dal zoznam
/// zobraziť naprieč všetkými psami bez ďalšieho dotazu.
/// </summary>
public class RecordListItemDto : BaseDto
{
	public int DogId { get; set; }

	public string DogName { get; set; }

	public RecordTypeEnum Type { get; set; }

	public DateTime OccurredOn { get; set; }

	public string Title { get; set; }

	public string Description { get; set; }

	public string VetName { get; set; }

	public decimal? Cost { get; set; }

	public decimal? WeightKg { get; set; }

	public DateTime? NextDueOn { get; set; }

	public int? FollowUpOfRecordId { get; set; }

	/// <summary>
	/// True, ak už existuje záznam, ktorý „ďalší termín“ tohto záznamu vybavil.
	/// </summary>
	public bool IsNextDueCompleted { get; set; }
}
