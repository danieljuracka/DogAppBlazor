using System.Globalization;

namespace DogAppBlazor.Contracts.PackingList;

/// <summary>
/// Predvolené kategórie baliaceho zoznamu a poradie, v ktorom sa zobrazujú aj exportujú.
/// Vlastné kategórie používateľa nasledujú za predvolenými v abecednom poradí.
/// </summary>
public static class PackingCategories
{
	public const string Clothing = "Oblečenie";
	public const string HikingGear = "Výbava na turistiku";
	public const string HygieneAndHealth = "Hygiena a zdravie";
	public const string Electronics = "Elektronika";
	public const string DocumentsAndMoney = "Doklady a financie";
	public const string FoodAndDrinks = "Jedlo a pitie";

	public static readonly IReadOnlyList<string> Defaults = [Clothing, HikingGear, HygieneAndHealth, Electronics, DocumentsAndMoney, FoodAndDrinks];

	private static readonly StringComparer s_textComparer = StringComparer.Create(new CultureInfo("sk-SK"), CompareOptions.IgnoreCase);

	/// <summary>
	/// Porovná dve kategórie podľa poradia zobrazenia.
	/// </summary>
	public static int Compare(string category1, string category2)
	{
		int result = GetDefaultOrder(category1).CompareTo(GetDefaultOrder(category2));
		return (result != 0) ? result : s_textComparer.Compare(category1, category2);
	}

	/// <summary>
	/// Zoradí položky podľa kategórie a názvu.
	/// </summary>
	public static List<PackingItemDto> Sort(IEnumerable<PackingItemDto> items)
	{
		return items
			.Order(Comparer<PackingItemDto>.Create((item1, item2) =>
			{
				int result = Compare(item1.Category, item2.Category);
				return (result != 0) ? result : s_textComparer.Compare(item1.Name, item2.Name);
			}))
			.ToList();
	}

	private static int GetDefaultOrder(string category)
	{
		for (int i = 0; i < Defaults.Count; i++)
		{
			if (s_textComparer.Equals(Defaults[i], category))
			{
				return i;
			}
		}
		return Defaults.Count;
	}
}
