using DogAppBlazor.Contracts.Records;
using DogAppBlazor.Web.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace DogAppBlazor.Web.Client.Components;

public partial class RecordTimelineItem : ComponentBase
{
	[Parameter, EditorRequired] public RecordListItemDto Record { get; set; }

	[Parameter] public bool ShowDogName { get; set; }

	[Parameter] public EventCallback<RecordListItemDto> OnSelect { get; set; }

	[Parameter] public EventCallback<int> OnEdit { get; set; }

	[Parameter] public EventCallback<int> OnDelete { get; set; }

	/// <summary>
	/// Vyvolá sa po kliknutí na „Vybavené“ pri ďalšom termíne. Parametrom je Id tohto záznamu.
	/// </summary>
	[Parameter] public EventCallback<int> OnCompleteFollowUp { get; set; }

	private string MarkerStyle => $"--marker-color: {RecordTypeFormatter.GetColor(Record.Type)};";

	private string NextDueCssClass => (Record.NextDueOn.Value.Date < DateTime.Today)
		? "timeline-due timeline-due-overdue"
		: "timeline-due";

	private Task SelectAsync()
	{
		return OnSelect.InvokeAsync(Record);
	}

	private Task EditAsync()
	{
		return OnEdit.InvokeAsync(Record.Id);
	}

	private Task DeleteAsync()
	{
		return OnDelete.InvokeAsync(Record.Id);
	}

	private Task CompleteFollowUpAsync()
	{
		return OnCompleteFollowUp.InvokeAsync(Record.Id);
	}

	private async Task HandleKeyDownAsync(KeyboardEventArgs args)
	{
		if ((args.Key == "Enter") || (args.Key == " "))
		{
			await SelectAsync();
		}
	}
}
