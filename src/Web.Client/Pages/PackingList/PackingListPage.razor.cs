using System.Globalization;
using DogAppBlazor.Contracts;
using DogAppBlazor.Contracts.PackingList;
using DogAppBlazor.Web.Client.Components;
using DogAppBlazor.Web.Client.Services;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;

namespace DogAppBlazor.Web.Client.Pages.PackingList;

public partial class PackingListPage : ComponentBase
{
	[Inject] protected IPackingListFacade PackingListFacade { get; set; }

	[Inject] protected IHxMessageBoxService MessageBox { get; set; }

	[Inject] protected IHxMessengerService Messenger { get; set; }

	[Inject] protected FileDownloader FileDownloader { get; set; }

	private PackingItemEditModal _editModal;
	private List<PackingItemDto> _items;

	/// <summary>
	/// Kategórie, ktoré majú aspoň jednu položku - ponuka filtra.
	/// </summary>
	private List<string> _usedCategories = [];

	/// <summary>
	/// Predvolené aj použité kategórie - ponuka pri pridávaní a úprave položky.
	/// </summary>
	private List<string> _suggestedCategories = [.. PackingCategories.Defaults];

	private readonly HashSet<string> _collapsedCategories = [];

	// Filtre nemajú viditeľné popisky - čítačky obrazovky dostanú popis cez aria-label.
	private readonly Dictionary<string, object> _searchInputAttributes = new() { ["aria-label"] = "Hľadať položku" };
	private readonly Dictionary<string, object> _categorySelectAttributes = new() { ["aria-label"] = "Filter podľa kategórie" };

	private string _searchText;
	private string _categoryFilter;
	private PackingStatusFilter _statusFilter = PackingStatusFilter.All;

	private int TotalCount => _items?.Count ?? 0;

	private int PackedCount => _items?.Count(item => item.IsPacked) ?? 0;

	/// <summary>
	/// Zaokrúhľuje sa nadol, aby 100 % znamenalo naozaj všetko zbalené.
	/// </summary>
	private int ProgressPercent => (TotalCount == 0) ? 0 : (PackedCount * 100 / TotalCount);

	private string ProgressText => TotalCount switch
	{
		0 => "Zoznam je zatiaľ prázdny",
		_ when PackedCount == TotalCount => "Všetko je zbalené, môžeme vyraziť!",
		_ => $"Zbalené {PackedCount} z {TotalCount}"
	};

	/// <summary>
	/// True, ak je nastavený len filter stavu - prázdny výsledok potom znamená, že v danom stave nič nie je.
	/// </summary>
	private bool IsStatusOnlyFilter => String.IsNullOrWhiteSpace(_searchText) && (_categoryFilter is null);

	protected override async Task OnInitializedAsync()
	{
		await ReloadAsync();
	}

	private async Task ReloadAsync()
	{
		_items = await PackingListFacade.GetItemsAsync();

		_usedCategories = _items
			.Select(item => item.Category)
			.Distinct(StringComparer.CurrentCultureIgnoreCase)
			.ToList();

		_suggestedCategories = PackingCategories.Defaults
			.Concat(_usedCategories)
			.Distinct(StringComparer.CurrentCultureIgnoreCase)
			.Order(Comparer<string>.Create(PackingCategories.Compare))
			.ToList();

		// HxSelect vyžaduje, aby hodnota bola v ponuke - kategória mohla zmiznúť zmazaním poslednej položky.
		if ((_categoryFilter is not null) && !_usedCategories.Contains(_categoryFilter))
		{
			_categoryFilter = null;
		}
	}

	/// <summary>
	/// Položky zoskupené podľa kategórie po uplatnení všetkých filtrov. Položky sú už zoradené zo servera,
	/// zoskupenie poradie zachová. Počty zbalených položiek sa rátajú z celej kategórie.
	/// </summary>
	private List<PackingCategoryGroup> GetVisibleGroups()
	{
		// Hľadá sa bez ohľadu na diakritiku („cel“ nájde „Čelovka“). Slovenské porovnávanie berie „č“ ako samostatné
		// písmeno, preto sa používa invariantná kultúra.
		CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
		string searchText = _searchText?.Trim();

		return _items
			.GroupBy(item => item.Category, StringComparer.CurrentCultureIgnoreCase)
			.Select(group => new PackingCategoryGroup(
				group.Key,
				group
					.Where(item => (_categoryFilter is null) || String.Equals(item.Category, _categoryFilter, StringComparison.CurrentCultureIgnoreCase))
					.Where(item => String.IsNullOrEmpty(searchText) || (compareInfo.IndexOf(item.Name, searchText, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0))
					.Where(item => _statusFilter switch
					{
						PackingStatusFilter.NotPacked => !item.IsPacked,
						PackingStatusFilter.Packed => item.IsPacked,
						_ => true
					})
					.ToList(),
				group.Count(item => item.IsPacked),
				group.Count()))
			.Where(group => group.Items.Count > 0)
			.ToList();
	}

	private string GetStatusFilterText(PackingStatusFilter status)
	{
		return status switch
		{
			PackingStatusFilter.NotPacked => $"Nezbalené ({TotalCount - PackedCount})",
			PackingStatusFilter.Packed => $"Zbalené ({PackedCount})",
			_ => $"Všetky ({TotalCount})"
		};
	}

	private void SetStatusFilter(PackingStatusFilter status)
	{
		_statusFilter = status;
	}

	private void ClearFilters()
	{
		_searchText = null;
		_categoryFilter = null;
		_statusFilter = PackingStatusFilter.All;
	}

	private void ToggleCollapsed(string category)
	{
		if (!_collapsedCategories.Remove(category))
		{
			_collapsedCategories.Add(category);
		}
	}

	/// <summary>
	/// Uloží stav zbalenia. Checkbox ho v položke zmenil už pred volaním, aby sa progres prepočítal
	/// bez čakania na server. Keď uloženie zlyhá, stav sa vráti späť.
	/// </summary>
	private async Task SavePackedAsync(PackingItemDto item)
	{
		try
		{
			await PackingListFacade.SetItemPackedAsync(new PackingItemPackedDto
			{
				Id = item.Id,
				IsPacked = item.IsPacked
			});
		}
		catch
		{
			item.IsPacked = !item.IsPacked;
			throw;
		}
	}

	private async Task PackAllAsync()
	{
		await PackingListFacade.SetAllPackedAsync(Dto.FromValue(true));
		_items.ForEach(item => item.IsPacked = true);
		Messenger.AddInformation("Hotovo", "Všetky položky sú označené ako zbalené.");
	}

	private async Task ResetAsync()
	{
		MessageBoxButtons result = await MessageBox.ShowAsync(new MessageBoxRequest
		{
			Title = "Začať baliť odznova",
			Text = "Zrušiť označenie všetkých zbalených položiek? Položky v zozname zostanú.",
			Buttons = MessageBoxButtons.YesNo,
			PrimaryButton = MessageBoxButtons.No
		});

		if (result != MessageBoxButtons.Yes)
		{
			return;
		}

		await PackingListFacade.SetAllPackedAsync(Dto.FromValue(false));
		_items.ForEach(item => item.IsPacked = false);
		Messenger.AddInformation("Odznova", "Označenia sú zrušené, môžeš baliť odznova.");
	}

	private Task AddAsync(string category)
	{
		return _editModal.ShowNewAsync(category ?? _categoryFilter);
	}

	private Task EditAsync(PackingItemDto item)
	{
		return _editModal.ShowEditAsync(item);
	}

	private async Task DeleteAsync(PackingItemDto item)
	{
		MessageBoxButtons result = await MessageBox.ShowAsync(new MessageBoxRequest
		{
			Title = "Odstrániť položku",
			Text = $"Naozaj chceš odstrániť položku „{item.Name}“ zo zoznamu?",
			Buttons = MessageBoxButtons.YesNo,
			PrimaryButton = MessageBoxButtons.No
		});

		if (result != MessageBoxButtons.Yes)
		{
			return;
		}

		await PackingListFacade.DeleteItemAsync(Dto.FromValue(item.Id));
		Messenger.AddInformation("Odstránené", "Položka je odstránená zo zoznamu.");

		await ReloadAsync();
	}

	private async Task ExportAsync()
	{
		PackingListExportDto export = await PackingListFacade.ExportToExcelAsync();
		await FileDownloader.DownloadAsync(export.FileName, PackingListExportDto.ContentType, export.Content);

		Messenger.AddInformation("Exportované", $"Zoznam sa uložil do súboru {export.FileName}.");
	}

	private enum PackingStatusFilter
	{
		All,
		NotPacked,
		Packed
	}

	private record PackingCategoryGroup(string Category, List<PackingItemDto> Items, int PackedCount, int TotalCount);
}
