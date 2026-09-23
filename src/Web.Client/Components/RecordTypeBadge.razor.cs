using DogAppBlazor.Contracts.Records;
using DogAppBlazor.Web.Client.Services;
using Microsoft.AspNetCore.Components;

namespace DogAppBlazor.Web.Client.Components;

public partial class RecordTypeBadge : ComponentBase
{
	[Parameter, EditorRequired] public RecordTypeEnum Type { get; set; }

	private string GetStyle()
	{
		return $"--record-type-color: {RecordTypeFormatter.GetColor(Type)};";
	}
}
