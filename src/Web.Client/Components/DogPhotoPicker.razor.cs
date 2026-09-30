using DogAppBlazor.Contracts;
using DogAppBlazor.Contracts.Dogs;
using DogAppBlazor.Contracts.Photos;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace DogAppBlazor.Web.Client.Components;

public partial class DogPhotoPicker : ComponentBase
{
	private static readonly TimeSpan ResizeTimeout = TimeSpan.FromSeconds(15);

	[Inject] protected IPetPhotoFacade PetPhotoFacade { get; set; }

	[Inject] protected IHxMessengerService Messenger { get; set; }

	[Parameter] public string Label { get; set; } = "Fotka";

	/// <summary>
	/// Názov súboru s fotkou (<see cref="DogDto.PhotoFileName"/>). Null znamená bez fotky.
	/// </summary>
	[Parameter] public string Value { get; set; }

	[Parameter] public EventCallback<string> ValueChanged { get; set; }

	/// <summary>
	/// Meno a farba psa pre náhľad, kým fotka nie je.
	/// </summary>
	[Parameter] public string DogName { get; set; }

	[Parameter] public string DogColor { get; set; }

	/// <summary>
	/// Počas nahrávania je true - formulár by sa medzitým nemal ukladať.
	/// </summary>
	[Parameter] public bool IsUploading { get; set; }

	[Parameter] public EventCallback<bool> IsUploadingChanged { get; set; }

	private string GetUploadButtonCssClass()
	{
		return IsUploading
			? "btn btn-outline-primary d-inline-flex align-items-center gap-2 disabled"
			: "btn btn-outline-primary d-inline-flex align-items-center gap-2";
	}

	private async Task HandleFileSelectedAsync(InputFileChangeEventArgs e)
	{
		if (!e.File.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
		{
			ShowUnreadableFileError();
			return;
		}

		await SetUploadingAsync(true);
		try
		{
			byte[] content;
			try
			{
				// Prehliadač fotku zmenší a prevedie do JPEG ešte pred prenosom - na server ide len pár stoviek kB.
				// Obrázok, ktorý prehliadač nevie dekódovať (napr. HEIC), sa nikdy nedokončí - preto timeout.
				IBrowserFile resized = await e.File
					.RequestImageFileAsync(PetPhotoUploadDto.ContentType, PetPhotoUploadDto.MaxDimension, PetPhotoUploadDto.MaxDimension)
					.AsTask()
					.WaitAsync(ResizeTimeout);
				await using Stream stream = resized.OpenReadStream(PetPhotoUploadDto.MaxSize);
				using MemoryStream memoryStream = new();
				await stream.CopyToAsync(memoryStream);
				content = memoryStream.ToArray();
			}
			catch (Exception ex) when (ex is IOException or InvalidOperationException or TimeoutException or Microsoft.JSInterop.JSException)
			{
				ShowUnreadableFileError();
				return;
			}

			Dto<string> fileName = await PetPhotoFacade.UploadPhotoAsync(new PetPhotoUploadDto { Content = content });
			Value = fileName.Value;
			await ValueChanged.InvokeAsync(Value);
		}
		finally
		{
			await SetUploadingAsync(false);
		}
	}

	private void ShowUnreadableFileError()
	{
		Messenger.AddError("Fotku sa nepodarilo načítať", "Vyber obrázok vo formáte JPEG, PNG alebo WebP.");
	}

	private async Task RemoveAsync()
	{
		Value = null;
		await ValueChanged.InvokeAsync(null);
	}

	private async Task SetUploadingAsync(bool isUploading)
	{
		IsUploading = isUploading;
		await IsUploadingChanged.InvokeAsync(isUploading);
	}
}
