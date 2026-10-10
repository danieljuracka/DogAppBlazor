namespace DogAppBlazor.Contracts.PackingList;

/// <summary>
/// Vygenerovaný súbor s baliacim zoznamom.
/// </summary>
public class PackingListExportDto
{
	public const string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

	public string FileName { get; set; }

	public byte[] Content { get; set; }
}
