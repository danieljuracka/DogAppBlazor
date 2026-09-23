namespace DogAppBlazor.Contracts.Records;

/// <summary>
/// Blížiaci sa alebo zmeškaný termín odvodený z <see cref="RecordDto.NextDueOn"/>.
/// </summary>
public class ReminderDto
{
	public int RecordId { get; set; }

	public int DogId { get; set; }

	public string DogName { get; set; }

	public RecordTypeEnum Type { get; set; }

	public string Title { get; set; }

	public DateTime DueOn { get; set; }

	/// <summary>
	/// True, ak je termín už po dátume.
	/// </summary>
	public bool IsOverdue { get; set; }
}
