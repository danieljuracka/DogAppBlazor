namespace DogAppBlazor.Contracts;

public class BaseDto
{
	/// <summary>
	/// Identifikator entity. Hodnota 0 znamena nový, este neulozeny zaznam.
	/// </summary>
	public int Id { get; set; }

	/// <summary>
	/// Stav objektu - pouziva sa pri editacii kolekcii na rozlisenie,
	/// ci bol zaznam pridany, zmeneny alebo zmazany.
	/// </summary>
	public EObjectStatus ObjectStatus { get; set; } = EObjectStatus.None;
}
