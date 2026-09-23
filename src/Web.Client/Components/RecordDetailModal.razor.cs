using DogAppBlazor.Contracts.Records;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;

namespace DogAppBlazor.Web.Client.Components;

/// <summary>
/// Read-only náhľad záznamu. Otvára sa klikom na záznam v zozname.
/// </summary>
public partial class RecordDetailModal : ComponentBase
{
	/// <summary>
	/// Vyvolá sa po zatvorení náhľadu, keď používateľ zvolil úpravu.
	/// Čaká sa na zatvorenie, aby sa dve modálne okná neprekrývali.
	/// </summary>
	[Parameter] public EventCallback<int> OnEditRequested { get; set; }

	private HxModal _modal;
	private RecordListItemDto _record;
	private int? _pendingEditRecordId;

	public async Task ShowAsync(RecordListItemDto record)
	{
		_record = record;
		_pendingEditRecordId = null;

		StateHasChanged();
		await _modal.ShowAsync();
	}

	public Task HideAsync()
	{
		return _modal.HideAsync();
	}

	private async Task RequestEditAsync()
	{
		_pendingEditRecordId = _record.Id;
		await _modal.HideAsync();
	}

	private async Task HandleClosedAsync()
	{
		int? recordId = _pendingEditRecordId;
		_pendingEditRecordId = null;
		_record = null;

		if (recordId is not null)
		{
			await OnEditRequested.InvokeAsync(recordId.Value);
		}
	}

	private string GetNextDueCssClass()
	{
		return (_record.NextDueOn.Value.Date < DateTime.Today)
			? "timeline-due timeline-due-overdue"
			: "timeline-due";
	}

	private static string GetText(string value)
	{
		return String.IsNullOrWhiteSpace(value) ? "—" : value;
	}

	private static string GetAmount(decimal? value, string unit)
	{
		return (value is null) ? "—" : $"{value.Value.ToString("0.##")} {unit}";
	}
}
