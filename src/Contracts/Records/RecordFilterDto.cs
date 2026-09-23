namespace DogAppBlazor.Contracts.Records;

/// <summary>
/// Filter zoznamu záznamov. Nevyplnené položky sa ignorujú.
/// </summary>
public class RecordFilterDto
{
	public int? DogId { get; set; }

	public RecordTypeEnum? Type { get; set; }
}
