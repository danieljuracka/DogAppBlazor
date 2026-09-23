using DogAppBlazor.Web.Client.Services;
using Microsoft.AspNetCore.Components;

namespace DogAppBlazor.Web.Client.Components;

public partial class DogColorPicker : ComponentBase
{
	[Parameter] public string Label { get; set; } = "Farba";

	/// <summary>
	/// Vybraná farba vo formáte #rrggbb.
	/// </summary>
	[Parameter] public string Value { get; set; }

	[Parameter] public EventCallback<string> ValueChanged { get; set; }

	private bool IsSelected(DogColor color)
	{
		return String.Equals(color.Hex, Value, StringComparison.OrdinalIgnoreCase);
	}

	private string GetSwatchCssClass(DogColor color)
	{
		return IsSelected(color) ? "dog-color-swatch selected" : "dog-color-swatch";
	}

	private async Task SelectAsync(DogColor color)
	{
		Value = color.Hex;
		await ValueChanged.InvokeAsync(color.Hex);
	}
}
