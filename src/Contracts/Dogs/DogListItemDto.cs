namespace DogAppBlazor.Contracts.Dogs;

/// <summary>
/// Polozka zoznamu psov.
/// </summary>
public class DogListItemDto : BaseDto
{
	public string Name { get; set; }

	public string Breed { get; set; }

	public SexEnum? Sex { get; set; }

	public DateTime? BirthDate { get; set; }

	public string PhotoFileName { get; set; }

	public string Color { get; set; }
}
