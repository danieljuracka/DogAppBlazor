using Havit.Blazor.Components.Web.Bootstrap;

namespace DogAppBlazor.Web.Client.Services;

/// <summary>
/// Celoaplikačné predvolby Havit komponentov. Volá sa zo servera aj z WebAssembly,
/// lebo ide o statický stav a každé prostredie si ho nastavuje samo.
/// </summary>
public static class HavitBlazorDefaults
{
	public static void Configure()
	{
		// Komponenty sú predvolene anglické, aplikácia je jednojazyčne slovenská.
		HxMessageBox.Defaults.YesButtonText = "Áno";
		HxMessageBox.Defaults.NoButtonText = "Nie";
		HxMessageBox.Defaults.OkButtonText = "OK";
		HxMessageBox.Defaults.CancelButtonText = "Zrušiť";
		HxMessageBox.Defaults.AbortButtonText = "Prerušiť";
		HxMessageBox.Defaults.RetryButtonText = "Skúsiť znova";
		HxMessageBox.Defaults.IgnoreButtonText = "Ignorovať";
	}
}
