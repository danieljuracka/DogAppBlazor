using DogAppBlazor.Contracts.Dogs;
using Microsoft.AspNetCore.Components;

namespace DogAppBlazor.Web.Client.Components;

public partial class DogEditForm : ComponentBase
{
	private static readonly SexEnum[] Sexes = [SexEnum.Male, SexEnum.Female];

	[Parameter, EditorRequired] public DogDto Dog { get; set; }

	/// <summary>
	/// Vyvolá sa po úspešnej validácii formulára.
	/// </summary>
	[Parameter] public EventCallback OnValidSubmit { get; set; }

	[Parameter] public EventCallback OnCancel { get; set; }

	/// <summary>
	/// Počas ukladania sú tlačidlá neaktívne.
	/// </summary>
	[Parameter] public bool IsBusy { get; set; }

	private Task HandleValidSubmitAsync()
	{
		return OnValidSubmit.InvokeAsync();
	}
}
