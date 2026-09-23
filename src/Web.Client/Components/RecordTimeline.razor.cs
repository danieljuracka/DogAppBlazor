using DogAppBlazor.Contracts.Records;
using Microsoft.AspNetCore.Components;

namespace DogAppBlazor.Web.Client.Components;

public partial class RecordTimeline : ComponentBase
{
	[Parameter, EditorRequired] public IReadOnlyList<RecordListItemDto> Records { get; set; } = [];

	/// <summary>
	/// Zobraziť meno psa pri každom zázname - používa sa v prehľade naprieč psami.
	/// </summary>
	[Parameter] public bool ShowDogName { get; set; }

	[Parameter] public string EmptyTitle { get; set; } = "Zatiaľ žiadne záznamy";

	[Parameter] public string EmptyText { get; set; } = "Pridaj prvý záznam a bude sa mať čo zobrazovať.";

	/// <summary>
	/// Vyvolá sa po kliknutí na záznam - otvára read-only náhľad.
	/// </summary>
	[Parameter] public EventCallback<RecordListItemDto> OnSelect { get; set; }

	[Parameter] public EventCallback<int> OnEdit { get; set; }

	[Parameter] public EventCallback<int> OnDelete { get; set; }

	[Parameter] public EventCallback<int> OnCompleteFollowUp { get; set; }
}
