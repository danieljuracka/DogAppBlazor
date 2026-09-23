using DogAppBlazor.Contracts.Records;
using DogAppBlazor.Web.Client.Services;
using Microsoft.AspNetCore.Components;

namespace DogAppBlazor.Web.Client.Components;

public partial class ReminderListItem : ComponentBase
{
	[Parameter, EditorRequired] public ReminderDto Reminder { get; set; }

	/// <summary>
	/// Vyvolá sa po kliknutí na „Vybavené“. Parametrom je Id záznamu s týmto termínom.
	/// </summary>
	[Parameter] public EventCallback<int> OnComplete { get; set; }

	private string DotStyle => $"--marker-color: {RecordTypeFormatter.GetColor(Reminder.Type)};";

	private string DueCssClass => Reminder.IsOverdue
		? "reminder-due reminder-due-overdue"
		: "reminder-due";

	private Task CompleteAsync()
	{
		return OnComplete.InvokeAsync(Reminder.RecordId);
	}
}
