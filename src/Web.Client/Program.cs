using System.Globalization;
using DogAppBlazor.Contracts;
using DogAppBlazor.Web.Client.Services;
using DogAppBlazor.Web.Client.Services.DataStores;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Havit.Blazor.Grpc.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Aplikacia je jednojazycna - datumy a cisla vzdy v slovenskom formate.
CultureInfo culture = new("sk-SK");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

HavitBlazorDefaults.Configure();

builder.Services.AddHxServices();
builder.Services.AddHxMessenger();
builder.Services.AddHxMessageBoxHost();

builder.Services.AddScoped<IDogsDataStore, DogsDataStore>();

// Fasady sa vo WebAssembly volaju cez gRPC-Web. Rozhrania s [ApiContract]
// sa najdu v assembly kontraktov a zaregistruju sa ako klientske proxy.
builder.Services.AddGrpcClientInfrastructure(assemblyToScanForDataContracts: typeof(Dto).Assembly);
builder.Services.AddGrpcClientsByApiContractAttributes(typeof(Dto).Assembly);

await builder.Build().RunAsync();
