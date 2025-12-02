using Firmness.Application.DTOs;

namespace Firmness.Application.Services;

public interface IExcelService
{
    Task<List<ExcelDataRow>> ReadSalesFileAsync(Stream fileStream);
}