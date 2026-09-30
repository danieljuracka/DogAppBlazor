namespace DogAppBlazor.Contracts.Records;

/// <summary>
/// Jedno váženie psa - bod v grafe vývoja hmotnosti.
/// </summary>
public class WeightPointDto
{
	/// <summary>
	/// Záznam, z ktorého hmotnosť pochádza.
	/// </summary>
	public int RecordId { get; set; }

	public DateTime Date { get; set; }

	public decimal WeightKg { get; set; }
}
