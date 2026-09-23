using DogAppBlazor.Contracts.Records;
using Microsoft.AspNetCore.Components;

namespace DogAppBlazor.Web.Client.Components;

public partial class RemindersCard : ComponentBase
{
	[Parameter, EditorRequired] public IReadOnlyList<ReminderDto> Reminders { get; set; } = [];

	[Parameter] public EventCallback<int> OnComplete { get; set; }
}
