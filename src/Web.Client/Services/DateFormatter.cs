namespace DogAppBlazor.Web.Client.Services;

/// <summary>
/// Formátovanie dátumov a termínov v texte, ktorý sa číta ako veta.
/// </summary>
public static class DateFormatter
{
	public static string ToShortDate(DateTime date)
	{
		return date.ToString("d. M. yyyy");
	}

	/// <summary>
	/// Vráti relatívny popis termínu, napr. "o 12 dní", "dnes", "pred 20 dňami".
	/// </summary>
	public static string ToRelative(DateTime date)
	{
		int days = (int)(date.Date - DateTime.Today).TotalDays;

		return days switch
		{
			0 => "dnes",
			1 => "zajtra",
			-1 => "včera",
			> 0 => $"o {days} {GetDayWord(days)}",
			_ => $"pred {-days} {GetDayWordAgo(-days)}"
		};
	}

	private static string GetDayWord(int count)
	{
		return (count >= 2) && (count <= 4) ? "dni" : "dní";
	}

	private static string GetDayWordAgo(int count)
	{
		return (count == 1) ? "dňom" : "dňami";
	}
}
