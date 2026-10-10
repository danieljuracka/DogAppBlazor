namespace DogAppBlazor.Facades.Storage;

/// <summary>
/// Názvy tabuliek v Azure Table Storage.
/// </summary>
public static class TableNames
{
	public const string Dogs = "Dogs";
	public const string Records = "Records";
	public const string PackingItems = "PackingItems";

	/// <summary>
	/// Počítadlá pre prideľovanie číselných Id - Table Storage nemá autoinkrement.
	/// </summary>
	public const string Counters = "Counters";

	public static readonly string[] All = [Dogs, Records, PackingItems, Counters];
}
