using DogAppBlazor.Contracts.PackingList;
using Microsoft.AspNetCore.Components;

namespace DogAppBlazor.Web.Client.Components;

public partial class PackingCategoryCard : ComponentBase
{
	[Parameter, EditorRequired] public string Category { get; set; }

	/// <summary>
	/// Zobrazené položky kategórie - už prefiltrované.
	/// </summary>
	[Parameter, EditorRequired] public IReadOnlyList<PackingItemDto> Items { get; set; } = [];

	/// <summary>
	/// Počet zbalených položiek celej kategórie bez ohľadu na filter.
	/// </summary>
	[Parameter] public int PackedCount { get; set; }

	/// <summary>
	/// Počet všetkých položiek kategórie bez ohľadu na filter.
	/// </summary>
	[Parameter] public int TotalCount { get; set; }

	[Parameter] public bool IsCollapsed { get; set; }

	[Parameter] public EventCallback OnToggleCollapsed { get; set; }

	/// <summary>
	/// Vyvolá sa po zmene checkboxu - stav zbalenia je už v položke zmenený a treba ho uložiť.
	/// </summary>
	[Parameter] public EventCallback<PackingItemDto> OnPackedChanged { get; set; }

	/// <summary>
	/// Pridanie novej položky do tejto kategórie.
	/// </summary>
	[Parameter] public EventCallback<string> OnAdd { get; set; }

	[Parameter] public EventCallback<PackingItemDto> OnEdit { get; set; }

	[Parameter] public EventCallback<PackingItemDto> OnDelete { get; set; }

	private string CountCssClass => ((TotalCount > 0) && (PackedCount == TotalCount))
		? "packing-category-count packing-category-count-done"
		: "packing-category-count";

	private static string GetItemCssClass(PackingItemDto item)
	{
		return item.IsPacked ? "packing-item packing-item-packed" : "packing-item";
	}
}
