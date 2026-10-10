using DogAppBlazor.Contracts.PackingList;

namespace DogAppBlazor.Facades.PackingList;

/// <summary>
/// Počiatočné položky baliaceho zoznamu. Vložia sa len raz, potom si ich používateľ upravuje sám.
/// </summary>
public static class PackingListDefaults
{
	public static readonly IReadOnlyList<(string Category, string Name)> Items =
	[
		(PackingCategories.Clothing, "Turistické topánky"),
		(PackingCategories.Clothing, "Funkčné tričká"),
		(PackingCategories.Clothing, "Nohavice na turistiku"),
		(PackingCategories.Clothing, "Náhradné ponožky"),
		(PackingCategories.Clothing, "Bunda do dažďa"),
		(PackingCategories.Clothing, "Teplá vrstva oblečenia"),

		(PackingCategories.HikingGear, "Turistický batoh"),
		(PackingCategories.HikingGear, "Fľaša na vodu"),
		(PackingCategories.HikingGear, "Čelovka"),
		(PackingCategories.HikingGear, "Turistické paličky"),
		(PackingCategories.HikingGear, "Mapa alebo navigácia"),
		(PackingCategories.HikingGear, "Slnečné okuliare"),
		(PackingCategories.HikingGear, "Ochrana pred slnkom"),

		(PackingCategories.HygieneAndHealth, "Zubná kefka a pasta"),
		(PackingCategories.HygieneAndHealth, "Dezodorant"),
		(PackingCategories.HygieneAndHealth, "Osobné lieky"),
		(PackingCategories.HygieneAndHealth, "Malá lekárnička"),
		(PackingCategories.HygieneAndHealth, "Opaľovací krém"),
		(PackingCategories.HygieneAndHealth, "Repelent"),

		(PackingCategories.Electronics, "Mobilný telefón"),
		(PackingCategories.Electronics, "Nabíjačka"),
		(PackingCategories.Electronics, "Powerbanka"),
		(PackingCategories.Electronics, "Slúchadlá"),
		(PackingCategories.Electronics, "Nabíjacie káble"),

		(PackingCategories.DocumentsAndMoney, "Občiansky preukaz alebo pas"),
		(PackingCategories.DocumentsAndMoney, "Platobná karta"),
		(PackingCategories.DocumentsAndMoney, "Hotovosť"),
		(PackingCategories.DocumentsAndMoney, "Poistenie"),
		(PackingCategories.DocumentsAndMoney, "Potvrdenia o rezerváciách"),

		(PackingCategories.FoodAndDrinks, "Voda"),
		(PackingCategories.FoodAndDrinks, "Desiata"),
		(PackingCategories.FoodAndDrinks, "Energetická tyčinka"),
		(PackingCategories.FoodAndDrinks, "Termoska")
	];
}
