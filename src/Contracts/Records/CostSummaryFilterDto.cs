namespace DogAppBlazor.Contracts.Records;

/// <summary>
/// Filter prehľadu nákladov. Nevyplnené položky sa ignorujú.
/// </summary>
public class CostSummaryFilterDto
{
	public int? DogId { get; set; }

	/// <summary>
	/// Rok, za ktorý sa náklady sčítajú. Null znamená celé obdobie.
	/// </summary>
	public int? Year { get; set; }
}
