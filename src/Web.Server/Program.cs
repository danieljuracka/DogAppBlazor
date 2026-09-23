using DogAppBlazor.Contracts;
using DogAppBlazor.Facades;
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

var app = builder.Build();

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

app.Run();
