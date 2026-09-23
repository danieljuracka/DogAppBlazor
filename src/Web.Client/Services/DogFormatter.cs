using DogAppBlazor.Contracts.Dogs;

namespace DogAppBlazor.Web.Client.Services;

/// <summary>
/// Formatovanie udajov o psovi pre zobrazenie v UI.
/// </summary>
public static class DogFormatter
{
	public static string GetSexText(SexEnum? sex)
	{
		return sex switch
		{
			SexEnum.Male => "Pes",
			SexEnum.Female => "Sučka",
			_ => null
		};
	}

	/// <summary>
	/// Vrati vek psa v citatelnej podobe, napr. "6 rokov" alebo "8 mesiacov".
	/// Ak datum narodenia nie je znamy, vrati null.
	/// </summary>
	public static string GetAgeText(DateTime? birthDate)
	{
		if (birthDate is null)
		{
			return null;
		}

		DateTime today = DateTime.Today;
		DateTime birth = birthDate.Value.Date;

		if (birth > today)
		{
			return null;
		}

		int months = ((today.Year - birth.Year) * 12) + today.Month - birth.Month;
		if (today.Day < birth.Day)
		{
			months--;
		}

		if (months < 1)
		{
			int days = (int)(today - birth).TotalDays;
			return $"{days} {GetDayWord(days)}";
		}

		if (months < 12)
		{
			return $"{months} {GetMonthWord(months)}";
		}

		int years = months / 12;
		return $"{years} {GetYearWord(years)}";
	}

	private static string GetDayWord(int count)
	{
		return count == 1 ? "deň" : "dní";
	}

	private static string GetMonthWord(int count)
	{
		if (count == 1)
		{
			return "mesiac";
		}
		return (count <= 4) ? "mesiace" : "mesiacov";
	}

	private static string GetYearWord(int count)
	{
		if (count == 1)
		{
			return "rok";
		}
		return (count <= 4) ? "roky" : "rokov";
	}
}
