namespace DogAppBlazor.Contracts.PackingList;

/// <summary>
/// Zmena stavu zbalenia jednej položky. Mení sa len stav, ostatné údaje položky zostanú, ako sú.
/// </summary>
public class PackingItemPackedDto
{
	public int Id { get; set; }

	public bool IsPacked { get; set; }
}
