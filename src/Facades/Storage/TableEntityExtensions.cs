using System.Globalization;
using Azure.Data.Tables;

namespace DogAppBlazor.Facades.Storage;

/// <summary>
/// Prevody hodnôt, ktoré Table Storage nepodporuje priamo alebo ich ukladá inak, než potrebujeme.
/// </summary>
public static class TableEntityExtensions
{
	/// <summary>
	/// Kľúč riadku z Id. Doplnenie nulami zaručí, že abecedné poradie v tabuľke zodpovedá číselnému.
	/// </summary>
	public static string ToRowKey(int id)
	{
		return id.ToString("D10", CultureInfo.InvariantCulture);
	}

	public static int GetId(this TableEntity entity)
	{
		return Int32.Parse(entity.RowKey, CultureInfo.InvariantCulture);
	}

	/// <summary>
	/// Table Storage ukladá dátumy v UTC. Dátum sa preto uloží s nulovým posunom, aby sa
	/// pri čítaní nezmenil podľa časového pásma servera.
	/// </summary>
	public static DateTimeOffset? ToStorageDate(DateTime? value)
	{
		return (value is null) ? null : new DateTimeOffset(DateTime.SpecifyKind(value.Value, DateTimeKind.Unspecified), TimeSpan.Zero);
	}

	public static DateTime? GetDate(this TableEntity entity, string key)
	{
		return entity.GetDateTimeOffset(key)?.DateTime;
	}

	/// <summary>
	/// Table Storage nepodporuje decimal - ukladá sa ako text, aby sa nestratila presnosť.
	/// </summary>
	public static string ToStorageDecimal(decimal? value)
	{
		return value?.ToString(CultureInfo.InvariantCulture);
	}

	public static decimal? GetDecimal(this TableEntity entity, string key)
	{
		string value = entity.GetString(key);
		return (value is null) ? null : Decimal.Parse(value, CultureInfo.InvariantCulture);
	}

	/// <summary>
	/// Enum sa ukladá ako názov hodnoty, aby bol v tabuľke čitateľný.
	/// </summary>
	public static TEnum? GetEnum<TEnum>(this TableEntity entity, string key)
		where TEnum : struct, Enum
	{
		string value = entity.GetString(key);
		return (value is null) ? null : Enum.Parse<TEnum>(value);
	}
}
