using System.ComponentModel.DataAnnotations;

namespace DogAppBlazor.Contracts.Dogs;

/// <summary>
/// Profil psa - pouziva sa na zobrazenie detailu aj na editaciu.
/// </summary>
public class DogDto : BaseDto
{
	/// <summary>
	/// Povolený formát farby. Hodnota sa vkladá do inline štýlu, preto len #rrggbb.
	/// </summary>
	public const string ColorPattern = "^#[0-9a-fA-F]{6}$";

	[Required(ErrorMessage = "Meno je povinné.")]
	[MaxLength(50)]
	public string Name { get; set; }

	[MaxLength(100)]
	public string Breed { get; set; }

	public SexEnum? Sex { get; set; }

	public DateTime? BirthDate { get; set; }

	[MaxLength(20)]
	public string MicrochipNumber { get; set; }

	[MaxLength(2000)]
	public string Note { get; set; }

	/// <summary>
	/// Nazov suboru s fotkou v ulozisku. Null znamena, ze pes fotku nema.
	/// </summary>
	[MaxLength(100)]
	public string PhotoFileName { get; set; }

	/// <summary>
	/// Farba psa vo formáte #rrggbb - základ jeho karty a avatara.
	/// </summary>
	[RegularExpression(ColorPattern, ErrorMessage = "Neplatná farba.")]
	public string Color { get; set; }
}
