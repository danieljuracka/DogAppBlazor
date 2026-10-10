using Azure.Data.Tables;
using Azure.Storage.Blobs;
using DogAppBlazor.Contracts;
using DogAppBlazor.Contracts.Photos;
using DogAppBlazor.Facades;
using DogAppBlazor.Facades.Photos;
using DogAppBlazor.Facades.Storage;
using DogAppBlazor.Web.Server.Components;
using DogAppBlazor.Web.Client;
using DogAppBlazor.Web.Client.Services;
using DogAppBlazor.Web.Client.Services.DataStores;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Havit.Blazor.Grpc.Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents()
	.AddInteractiveWebAssemblyComponents();

builder.Services.AddHxServices();
builder.Services.AddHxMessenger();
builder.Services.AddHxMessageBoxHost();

HavitBlazorDefaults.Configure();

builder.Services.AddScoped<IDogsDataStore, DogsDataStore>();
builder.Services.AddScoped<FileDownloader>();

// Havit.Blazor.Grpc.Server pouziva IExceptionMonitoringService na hlasenie chyb fasad.
builder.Services.AddExceptionMonitoring(builder.Configuration);

// Fasady - registruju sa aj pre server-side rendering (prerendering a interaktivny Server rezim),
// kde sa volaju priamo v procese. WebAssembly ich vola cez gRPC-Web.
builder.Services.AddGrpcServerInfrastructure(assemblyToScanForDataContracts: typeof(Dto).Assembly);
builder.Services.AddFacadesByServiceAttribute();

// Azure Table Storage - connection string sa cita z konfiguracie
// (lokalne z User Secrets, v Azure z App Service > Environment variables > Connection strings).
string azureStorageConnectionString = builder.Configuration.GetConnectionString("AzureStorage");
if (String.IsNullOrWhiteSpace(azureStorageConnectionString))
{
	throw new InvalidOperationException("Chýba connection string 'AzureStorage'. Lokálne ho nastav cez: dotnet user-secrets set \"ConnectionStrings:AzureStorage\" \"<connection string>\" --project src/Web.Server");
}
builder.Services.AddSingleton(new TableServiceClient(azureStorageConnectionString));
builder.Services.AddSingleton(new BlobServiceClient(azureStorageConnectionString));

var app = builder.Build();

// Vytvori tabulky, ktore v Storage accounte este neexistuju.
await app.Services.GetRequiredService<TableStorageInitializer>().InitializeAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}
else
{
	app.UseExceptionHandler(NavigationRoutes.Error, createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRequestLocalization(new RequestLocalizationOptions()
	.SetDefaultCulture("sk-SK")
	.AddSupportedCultures("sk-SK")
	.AddSupportedUICultures("sk-SK"));

app.UseAntiforgery();

app.UseGrpcWeb(new GrpcWebOptions() { DefaultEnabled = true });

app.MapStaticAssets();
app.MapGrpcServicesByApiContractAttributes(typeof(Dto).Assembly);

// Fotky zvierat - blob kontajner je súkromný, fotky preto vydáva server.
// Názov fotky sa nikdy nemení (nová fotka = nový názov), takže sa môže cachovať natrvalo.
app.MapGet(NavigationRoutes.PetPhotos.Photo, async (string fileName, PetPhotoStorage petPhotoStorage, HttpContext httpContext, CancellationToken cancellationToken) =>
{
	Stream photo = await petPhotoStorage.OpenReadAsync(fileName, cancellationToken);
	if (photo is null)
	{
		return Results.NotFound();
	}

	httpContext.Response.Headers.CacheControl = "private, max-age=31536000, immutable";
	httpContext.Response.Headers.XContentTypeOptions = "nosniff";
	return Results.Stream(photo, PetPhotoUploadDto.ContentType);
});
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode()
	.AddInteractiveWebAssemblyRenderMode()
	.AddAdditionalAssemblies(typeof(DogAppBlazor.Web.Client._Imports).Assembly);

await app.RunAsync();
