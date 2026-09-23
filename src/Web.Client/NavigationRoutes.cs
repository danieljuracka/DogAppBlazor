namespace DogAppBlazor.Web.Client;

/// <summary>
/// Všetky URI aplikácie na jednom mieste. Stránky ich používajú cez
/// <c>@attribute [Route(...)]</c>, navigácia cez <c>Get*</c> metódy.
/// </summary>
public static class NavigationRoutes
{
	#region General

	public const string Home = "/";

	/// <summary>
	/// Chybová stránka servera, cieľ pre UseExceptionHandler.
	/// </summary>
	public const string Error = "/Error";

	#endregion

	#region Dogs

	public static class Dogs
	{
		/// <summary>
		/// Zoznam psov je zároveň úvodná stránka.
		/// </summary>
		public const string Index = Home;

		public const string Create = "/pes/novy";
		public const string Detail = "/pes/{Id:int}";
		public const string Edit = "/pes/{Id:int}/upravit";

		public static string GetDetail(int dogId) => $"/pes/{dogId}";

		public static string GetEdit(int dogId) => $"/pes/{dogId}/upravit";
	}

	#endregion

	#region Records

	public static class Records
	{
		public const string Index = "/zaznamy";
	}

	#endregion
}
