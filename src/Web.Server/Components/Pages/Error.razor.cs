using System.Diagnostics;
using Microsoft.AspNetCore.Components;

namespace DogAppBlazor.Web.Server.Components.Pages;

public partial class Error : ComponentBase
{
	[CascadingParameter] private HttpContext HttpContext { get; set; }

	private string RequestId { get; set; }

	private bool ShowRequestId => !String.IsNullOrEmpty(RequestId);

	protected override void OnInitialized()
	{
		RequestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier;
	}
}
