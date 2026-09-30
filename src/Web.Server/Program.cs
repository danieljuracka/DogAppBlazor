using Azure.Data.Tables;
using DogAppBlazor.Contracts;
using DogAppBlazor.Facades;
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
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode()
	.AddInteractiveWebAssemblyRenderMode()
	.AddAdditionalAssemblies(typeof(DogAppBlazor.Web.Client._Imports).Assembly);

await app.RunAsync();
