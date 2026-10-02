namespace DogAppBlazor.Web.Client.Services;

/// <summary>
/// Formátovanie súm a období v prehľade nákladov.
/// </summary>
public static class CostFormatter
{
	private static readonly string[] _monthShortNames = ["jan", "feb", "mar", "apr", "máj", "jún", "júl", "aug", "sep", "okt", "nov", "dec"];

	private static readonly string[] _monthNames = ["január", "február", "marec", "apríl", "máj", "jún", "júl", "august", "september", "október", "november", "december"];

	/// <summary>
	/// Suma v eurách v slovenskom formáte, napr. "1 234,50 €".
	/// </summary>
	public static string ToMoney(decimal value)
	{
		return value.ToString("C2");
	}

	public static string GetMonthShortName(int month)
	{
		return _monthShortNames[month - 1];
	}

	public static string GetMonthName(int month)
	{
		return _monthNames[month - 1];
	}

	/// <summary>
	/// Text "1 záznam", "3 záznamy", "5 záznamov".
	/// </summary>
	public static string GetRecordCountText(int count)
	{
		return count switch
		{
			1 => "1 záznam",
			>= 2 and <= 4 => $"{count} záznamy",
			_ => $"{count} záznamov"
		};
	}
}
