using DogAppBlazor.Contracts;
using DogAppBlazor.Contracts.PackingList;
using Havit.Extensions.DependencyInjection.Abstractions;

namespace DogAppBlazor.Facades.PackingList;

[Service]
public class PackingListFacade(PackingItemStorage packingItemStorage) : IPackingListFacade
{
	private const string ExportFileName = "baliaci_zoznam.xlsx";

	private readonly PackingItemStorage _packingItemStorage = packingItemStorage;

	public async Task<List<PackingItemDto>> GetItemsAsync(CancellationToken cancellationToken = default)
	{
		return PackingCategories.Sort(await _packingItemStorage.GetAllAsync(cancellationToken));
	}

	public async Task<Dto<int>> UpdateItemAsync(PackingItemDto packingItemDto, CancellationToken cancellationToken = default)
	{
		Contract.Requires<ArgumentNullException>(packingItemDto is not null);
		Contract.Requires<ArgumentException>(!String.IsNullOrWhiteSpace(packingItemDto.Name));
		Contract.Requires<ArgumentException>(!String.IsNullOrWhiteSpace(packingItemDto.Category));

		packingItemDto.Name = packingItemDto.Name.Trim();
		packingItemDto.Category = packingItemDto.Category.Trim();
		packingItemDto.Note = String.IsNullOrWhiteSpace(packingItemDto.Note) ? null : packingItemDto.Note.Trim();

		return Dto.FromValue(await _packingItemStorage.UpsertAsync(packingItemDto, cancellationToken));
	}

	public async Task SetItemPackedAsync(PackingItemPackedDto packingItemPackedDto, CancellationToken cancellationToken = default)
	{
		Contract.Requires<ArgumentNullException>(packingItemPackedDto is not null);

		await _packingItemStorage.SetPackedAsync(packingItemPackedDto.Id, packingItemPackedDto.IsPacked, cancellationToken);
	}

	public async Task SetAllPackedAsync(Dto<bool> isPacked, CancellationToken cancellationToken = default)
	{
		Contract.Requires<ArgumentNullException>(isPacked is not null);

		await _packingItemStorage.SetAllPackedAsync(isPacked.Value, cancellationToken);
	}

	public async Task DeleteItemAsync(Dto<int> id, CancellationToken cancellationToken = default)
	{
		Contract.Requires<ArgumentNullException>(id is not null);

		await _packingItemStorage.DeleteAsync(id.Value, cancellationToken);
	}

	public async Task<PackingListExportDto> ExportToExcelAsync(CancellationToken cancellationToken = default)
	{
		List<PackingItemDto> items = await GetItemsAsync(cancellationToken);

		return new PackingListExportDto
		{
			FileName = ExportFileName,
			Content = PackingListExcelWriter.Write(items)
		};
	}
}
