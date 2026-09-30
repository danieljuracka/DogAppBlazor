namespace DogAppBlazor.Contracts.Photos;

/// <summary>
/// Nahrávaná fotka (psa, iného zvieraťa, čohokoľvek). Klient ju pred odoslaním zmenší a prevedie do JPEG.
/// </summary>
public class PetPhotoUploadDto
{
	/// <summary>
	/// Jediný podporovaný formát - server fotky vydáva vždy ako image/jpeg.
	/// </summary>
	public const string ContentType = "image/jpeg";

	/// <summary>
	/// Maximálna veľkosť už zmenšenej fotky v bajtoch.
	/// Musí sa zmestiť do limitu gRPC správy (4 MB).
	/// </summary>
	public const int MaxSize = 2 * 1024 * 1024;

	/// <summary>
	/// Maximálny rozmer (šírka aj výška) fotky v pixeloch po zmenšení na klientovi.
	/// </summary>
	public const int MaxDimension = 800;

	public byte[] Content { get; set; }
}
