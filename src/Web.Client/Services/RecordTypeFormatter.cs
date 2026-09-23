using DogAppBlazor.Contracts.Records;

namespace DogAppBlazor.Web.Client.Services;

/// <summary>
/// Pomenovanie a farba druhov záznamov pre UI.
/// </summary>
public static class RecordTypeFormatter
{
	/// <summary>
	/// Druhy v poradí, v akom sa ponúkajú vo formulári a vo filtri.
	/// </summary>
	public static readonly RecordTypeEnum[] AllTypes =
	[
		RecordTypeEnum.VetVisit,
		RecordTypeEnum.Vaccination,
		RecordTypeEnum.Symptom,
		RecordTypeEnum.Deworming,
		RecordTypeEnum.Medication,
		RecordTypeEnum.Weight,
		RecordTypeEnum.Grooming,
		RecordTypeEnum.Other
	];

	public static string GetLabel(RecordTypeEnum type)
	{
		return type switch
		{
			RecordTypeEnum.VetVisit => "Veterinár",
			RecordTypeEnum.Vaccination => "Očkovanie",
			RecordTypeEnum.Symptom => "Príznak",
			RecordTypeEnum.Deworming => "Odčervenie",
			RecordTypeEnum.Medication => "Lieky",
			RecordTypeEnum.Weight => "Váženie",
			RecordTypeEnum.Grooming => "Starostlivosť",
			_ => "Iné"
		};
	}

	public static string GetColor(RecordTypeEnum type)
	{
		return type switch
		{
			RecordTypeEnum.VetVisit => "#4e9086",
			RecordTypeEnum.Vaccination => "#d9a123",
			RecordTypeEnum.Symptom => "#e8734c",
			RecordTypeEnum.Deworming => "#6b8f71",
			RecordTypeEnum.Medication => "#5b8296",
			RecordTypeEnum.Weight => "#b8893a",
			RecordTypeEnum.Grooming => "#a4614f",
			_ => "#75898f"
		};
	}
}
