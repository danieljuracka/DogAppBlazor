using DogAppBlazor.Web.Client.Services;
using Microsoft.AspNetCore.Components;

namespace DogAppBlazor.Web.Client.Components;

public partial class DogAvatar : ComponentBase
{
	private static readonly string[] Palette = ["#4e9086", "#c2643f", "#b8893a", "#6b8f71", "#a4614f", "#5b8296"];

	/// <summary>
	/// Meno psa - určuje iniciálu aj farbu pozadia.
	/// </summary>
	[Parameter, EditorRequired] public string Name { get; set; }

	/// <summary>
	/// Veľkosť avatara v pixeloch.
	/// </summary>
	[Parameter] public int Size { get; set; } = 48;

	/// <summary>
	/// Farba psa. Keď nie je zadaná, farba sa odvodí z mena.
	/// </summary>
	[Parameter] public string Color { get; set; }

	[Parameter] public string PhotoUrl { get; set; }

	/// <summary>
	/// Farba prstenca okolo avatara. Keď nie je zadaná, prstenec sa nekreslí.
	/// </summary>
	[Parameter] public string RingColor { get; set; }

	private string GetInitial()
	{
		return String.IsNullOrWhiteSpace(Name)
			? "?"
			: Name.Trim()[..1].ToUpperInvariant();
	}

	private string GetStyle()
	{
		string style = $"width: {Size}px; height: {Size}px; font-size: {Size / 2.3:0}px; background-color: {GetColor()};";

		if (!String.IsNullOrEmpty(RingColor))
		{
			style += $" --dog-avatar-ring: {DogColors.Normalize(RingColor)};";
		}

		return style;
	}

	private string GetColor()
	{
		if (!String.IsNullOrEmpty(Color))
		{
			return DogColors.Normalize(Color);
		}

		if (String.IsNullOrWhiteSpace(Name))
		{
			return Palette[0];
		}

		int hash = Name.Aggregate(0, (current, c) => current + c);
		return Palette[hash % Palette.Length];
	}
}
