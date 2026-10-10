using ClosedXML.Excel;
using DogAppBlazor.Contracts.PackingList;

namespace DogAppBlazor.Facades.PackingList;

/// <summary>
/// Zapíše baliaci zoznam do Excelu (.xlsx). Položky musia byť už zoradené.
/// </summary>
public static class PackingListExcelWriter
{
	private const string SheetName = "Baliaci zoznam";

	private static readonly string[] s_headers = ["Kategória", "Názov položky", "Zbalené", "Poznámka"];

	/// <summary>
	/// Minimálne a maximálne šírky stĺpcov v znakoch. Šírka sa prispôsobí obsahu v týchto medziach,
	/// dlhé poznámky sa zalamujú.
	/// </summary>
	private static readonly (double Min, double Max)[] s_columnWidths = [(14, 30), (18, 45), (10, 10), (20, 60)];

	private static readonly XLColor s_headerBackground = XLColor.FromHtml("#4E9086");
	private static readonly XLColor s_lineColor = XLColor.FromHtml("#F0E2D4");
	private static readonly XLColor s_packedColor = XLColor.FromHtml("#3F7A71");
	private static readonly XLColor s_notPackedColor = XLColor.FromHtml("#D05E39");

	public static byte[] Write(IReadOnlyList<PackingItemDto> items)
	{
		using XLWorkbook workbook = new XLWorkbook();
		IXLWorksheet sheet = workbook.Worksheets.Add(SheetName);

		for (int column = 0; column < s_headers.Length; column++)
		{
			sheet.Cell(1, column + 1).Value = s_headers[column];
		}

		IXLRange headerRange = sheet.Range(1, 1, 1, s_headers.Length);
		headerRange.Style.Font.Bold = true;
		headerRange.Style.Font.FontColor = XLColor.White;
		headerRange.Style.Fill.BackgroundColor = s_headerBackground;
		headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
		sheet.Row(1).Height = 22;

		int row = 2;
		foreach (PackingItemDto item in items)
		{
			sheet.Cell(row, 1).Value = item.Category;
			sheet.Cell(row, 2).Value = item.Name;
			sheet.Cell(row, 3).Value = item.IsPacked ? "Áno" : "Nie";
			sheet.Cell(row, 3).Style.Font.FontColor = item.IsPacked ? s_packedColor : s_notPackedColor;
			sheet.Cell(row, 4).Value = item.Note;
			row++;
		}

		int lastRow = Math.Max(row - 1, 1);
		IXLRange tableRange = sheet.Range(1, 1, lastRow, s_headers.Length);
		tableRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
		tableRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
		tableRange.Style.Border.BottomBorderColor = s_lineColor;
		sheet.Range(1, 3, lastRow, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
		sheet.Range(1, 4, lastRow, 4).Style.Alignment.WrapText = true;

		for (int column = 0; column < s_headers.Length; column++)
		{
			int contentLength = sheet.Column(column + 1).CellsUsed().Select(cell => cell.GetString().Length).DefaultIfEmpty(0).Max();
			sheet.Column(column + 1).Width = Math.Clamp(contentLength + 2, s_columnWidths[column].Min, s_columnWidths[column].Max);
		}

		tableRange.SetAutoFilter();
		sheet.SheetView.FreezeRows(1);

		using MemoryStream stream = new MemoryStream();
		workbook.SaveAs(stream);
		return stream.ToArray();
	}
}
