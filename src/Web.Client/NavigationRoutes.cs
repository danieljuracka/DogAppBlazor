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

		public const string Create = "/dog/new";
		public const string Detail = "/dog/{Id:int}";
		public const string Edit = "/dog/{Id:int}/edit";

		public static string GetDetail(int dogId) => $"/dog/{dogId}";

		public static string GetEdit(int dogId) => $"/dog/{dogId}/edit";
	}

	#endregion

	#region Records

	public static class Records
	{
		public const string Index = "/records";
	}

	#endregion

	#region Costs

	public static class Costs
	{
		public const string Index = "/costs";

		/// <summary>
		/// Query parameter s Id psa.
		/// </summary>
		public const string DogQueryName = "dog";

		/// <summary>
		/// Query parameter s rokom alebo hodnotou <see cref="AllYearsQueryValue"/>. Keď chýba, zobrazí sa aktuálny rok.
		/// </summary>
		public const string YearQueryName = "year";

		public const string AllYearsQueryValue = "all";

		public static string GetIndex(int dogId) => $"{Index}?{DogQueryName}={dogId}";
	}

	#endregion

	#region PackingList

	public static class PackingList
	{
		public const string Index = "/packing-list";
	}

	#endregion

	#region PetPhotos

	/// <summary>
	/// Fotky zvierat - nie sú to stránky, fotky vydáva endpoint servera.
	/// </summary>
	public static class PetPhotos
	{
		public const string Photo = "/pet-photos/{fileName}";

		/// <summary>
		/// URL fotky. Keď fotka nie je, vráti null.
		/// </summary>
		public static string GetPhoto(string photoFileName) => String.IsNullOrEmpty(photoFileName) ? null : $"/pet-photos/{Uri.EscapeDataString(photoFileName)}";
	}

	#endregion
}
