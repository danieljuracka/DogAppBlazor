using DogAppBlazor.Contracts.PackingList;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;

namespace DogAppBlazor.Web.Client.Components;

/// <summary>
/// Modálne okno na pridanie a úpravu položky baliaceho zoznamu.
/// </summary>
public partial class PackingItemEditModal : ComponentBase
{
	private const string CategoryListId = "packing-item-categories";

	[Inject] protected IPackingListFacade PackingListFacade { get; set; }

	[Inject] protected IHxMessengerService Messenger { get; set; }

	/// <summary>
	/// Kategórie ponúkané pri vypĺňaní.
	/// </summary>
	[Parameter] public IReadOnlyList<string> Categories { get; set; } = [];

	/// <summary>
	/// Vyvolá sa po úspešnom uložení, aby volajúci mohol znovu načítať zoznam.
	/// </summary>
	[Parameter] public EventCallback OnSaved { get; set; }

	private readonly Dictionary<string, object> _categoryInputAttributes = new() { ["list"] = CategoryListId };

	private HxModal _modal;
	private PackingItemDto _item;
	private string _title;
	private bool _isBusy;

	/// <summary>
	/// Otvorí okno pre novú položku. Category predvyplní kategóriu.
	/// </summary>
	public async Task ShowNewAsync(string category = null)
	{
		_item = new PackingItemDto
		{
			Category = category ?? PackingCategories.Defaults[0]
		};
		_title = "Nová položka";

		// Bez tohto sa HxModal zobrazí skôr, ako k nemu dorazí nový Title.
		StateHasChanged();
		await _modal.ShowAsync();
	}

	/// <summary>
	/// Otvorí okno s existujúcou položkou. Upravuje sa kópia, zoznam sa zmení až po uložení.
	/// </summary>
	public async Task ShowEditAsync(PackingItemDto item)
	{
		_item = new PackingItemDto
		{
			Id = item.Id,
			Name = item.Name,
			Category = item.Category,
			Note = item.Note,
			IsPacked = item.IsPacked
		};
		_title = "Úprava položky";

		StateHasChanged();
		await _modal.ShowAsync();
	}

	public Task HideAsync()
	{
		return _modal.HideAsync();
	}

	private async Task SaveAsync()
	{
		_isBusy = true;
		try
		{
			bool isNew = _item.Id == 0;
			await PackingListFacade.UpdateItemAsync(_item);
			Messenger.AddInformation("Uložené", isNew ? "Položka je pridaná do zoznamu." : "Položka je uložená.");

			await _modal.HideAsync();
			await OnSaved.InvokeAsync();
		}
		finally
		{
			_isBusy = false;
		}
	}

	private void HandleClosed()
	{
		_item = null;
	}
}
