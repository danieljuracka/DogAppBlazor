using System.ComponentModel.DataAnnotations;

namespace DogAppBlazor.Contracts.PackingList;

/// <summary>
/// Položka baliaceho zoznamu - vec, ktorú si treba zbaliť na turistiku alebo cestu.
/// </summary>
public class PackingItemDto : BaseDto
{
	[Required(ErrorMessage = "Názov je povinný.")]
	[MaxLength(100)]
	public string Name { get; set; }

	/// <summary>
	/// Kategória je voľný text. Predvolené kategórie sú v <see cref="PackingCategories"/>.
	/// </summary>
	[Required(ErrorMessage = "Kategória je povinná.")]
	[MaxLength(50)]
	public string Category { get; set; }

	[MaxLength(500)]
	public string Note { get; set; }

	public bool IsPacked { get; set; }
}
