using Firmness.Application.DTOs;

namespace Firmness.Application.Services;

public interface IExcelService
{
    /// Parses an Excel file stream and converts it into a list of data rows.
    /// <returns>A list of ExcelDataRow objects representing the file content.</returns>
    Task<List<ExcelDataRow>> ReadSalesFileAsync(Stream fileStream);
}