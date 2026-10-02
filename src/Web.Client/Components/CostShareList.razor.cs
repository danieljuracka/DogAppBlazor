using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace DogAppBlazor.Web.Client.Components;

public partial class CostShareList : ComponentBase
{
	/// <summary>
	/// Položky zoradené tak, ako sa majú zobraziť.
	/// </summary>
	[Parameter, EditorRequired] public IReadOnlyList<CostShareItem> Items { get; set; } = [];

	private decimal Total => Items.Sum(item => item.Total);

	/// <summary>
	/// Farba ide do inline štýlu, preto ju volajúci musí dodať v overenom formáte.
	/// </summary>
	private string GetStyle(CostShareItem item)
	{
		decimal percent = (Total == 0) ? 0 : Math.Round(item.Total / Total * 100, 1);
		return $"--share-color: {item.Color}; --share-percent: {percent.ToString(CultureInfo.InvariantCulture)}%;";
	}
}

/// <summary>
/// Jeden riadok rozpisu nákladov.
/// </summary>
/// <param name="Color">Farba vo formáte #rrggbb.</param>
/// <param name="Href">Nepovinný odkaz na detail kategórie.</param>
public record CostShareItem(string Label, string Color, decimal Total, int RecordCount, string Href = null);
