using System.Text.RegularExpressions;
using DogAppBlazor.Contracts.Dogs;

namespace DogAppBlazor.Web.Client.Services;

/// <summary>
/// Paleta farieb, z ktorých si pes vyberá. Každá je dosť tmavá na bielu iniciálu v avatare
/// a zároveň dáva po zmiešaní s bielou jemný podklad karty.
/// </summary>
public static class DogColors
{
	public const string Default = "#4e9086";

	public static readonly IReadOnlyList<DogColor> Palette =
	[
		new("#4e9086", "Tyrkysová"),
		new("#c48a1a", "Jantárová"),
		new("#d9623b", "Koralová"),
		new("#6b8f71", "Šalviová"),
		new("#4f7fa8", "Modrá"),
		new("#8a6fb0", "Levanduľová"),
		new("#c2607f", "Ružová"),
		new("#9a6b4f", "Gaštanová"),
		new("#c2702b", "Ginger"),
		new("#6e4a36", "Čokoládová"),
		new("#8e3b4a", "Bordová"),
		new("#2f6f7e", "Petrolejová"),
		new("#4a5258", "Grafitová")
	];

	/// <summary>
	/// Vráti farbu, ak má platný formát, inak predvolenú. Hodnota ide do inline štýlu,
	/// preto sa nikdy nepoužije bez overenia.
	/// </summary>
	public static string Normalize(string color)
	{
		return ((color is not null) && Regex.IsMatch(color, DogDto.ColorPattern))
			? color
			: Default;
	}

	/// <summary>
	/// Navrhne farbu pre nového psa - prvú z palety, ktorú ešte nikto nemá.
	/// </summary>
	public static string SuggestUnused(IEnumerable<string> usedColors)
	{
		HashSet<string> used = usedColors
			.Where(c => c is not null)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		return Palette.FirstOrDefault(c => !used.Contains(c.Hex))?.Hex ?? Default;
	}
}

public record DogColor(string Hex, string Name);
