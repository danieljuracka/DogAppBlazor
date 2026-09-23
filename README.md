# DogAppBlazor

Súkromná aplikácia na vedenie záznamov o našich psoch — profily, návštevy veterinára a zdravotné záznamy.

## Štruktúra

| Projekt | Obsah |
|---|---|
| `src/Contracts` | Rozhrania fasád označené `[ApiContract]`, DTO, `Dto<T>`, `BaseDto` |
| `src/Facades` | Implementácie fasád označené `[Service]` |
| `src/Web.Client` | Blazor WebAssembly — stránky, komponenty, `DataStores` |
| `src/Web.Server` | ASP.NET Core host, mapovanie gRPC fasád |

`Web.Client` referencuje iba `Contracts`, takže hranicu medzi frontendom a backendom drží kompilátor.

## Komunikácia FE ↔ BE

Fasády sú vystavené cez gRPC code-first (protobuf-net.Grpc + `Havit.Blazor.Grpc.*`). Rozhranie fasády je
zároveň kontrakt — klientske proxy sa generujú za behu, žiadne ručne písané endpointy.

Render mode je `InteractiveAuto`:

* pri prerenderingu a v interaktívnom Server režime sa fasáda volá priamo v procese,
* vo WebAssembly cez gRPC-Web.

## Štýly

Globálna téma sa píše v SCSS v `src/Web.Client/wwwroot/scss` a kompiluje do
`src/Web.Client/wwwroot/css/main.css`, ktorý je commitnutý v repozitári (build ani CI
nespúšťajú npm). Po každej zmene SCSS teda treba prekompilovať:

```bash
npm run compile:scss
```

Počas úprav sa hodí `npm run watch:scss`.

Scoped štýly komponentov zostávajú v `.razor.css` súboroch vedľa komponentov —
Blazor ich spracúva sám a do SCSS sa neprenášajú.

## Spustenie

```bash
dotnet run --project src/Web.Server/Web.Server.csproj --launch-profile https
```

Aplikácia beží na `https://localhost:7117`.

## Stav

Dáta sú zatiaľ dočasne v pamäti (`DogStorage`) a po reštarte sa vrátia do počiatočného stavu.
Napojenie na databázu je ďalší krok.
