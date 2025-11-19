using Firmness.Application.DTOs;
using OfficeOpenXml; 


namespace Firmness.Application.Services;

public class ExcelService : IExcelService
    {
        [Obsolete("Obsolete")]
        public ExcelService()
        {
          
            ExcelPackage.License.SetNonCommercialPersonal("maikol");
        }

        public async Task<List<ExcelDataRow>> ReadSalesFileAsync(Stream fileStream)
        {
            var rows = new List<ExcelDataRow>();

            using (var package = new ExcelPackage(fileStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                
                if (worksheet.Dimension == null) return rows;

                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var dto = new ExcelDataRow();
                    try
                    {
                        // Map columns by index (1-based in EPPlus)
                        // 1:Invoice, 2:Date, 3:Client Doc, 4:Client Name, 5:Email, 6:Address, 7:Phone, 8:ProdName, 9:Desc, 10:Price, 11:Qty
                        dto.InvoiceNumber = worksheet.Cells[row, 1].GetValue<int>();
                        dto.Date = worksheet.Cells[row, 2].GetValue<DateTime>();
                        
                        dto.ClientDocument = worksheet.Cells[row, 3].GetValue<string>()?.Trim();
                        dto.ClientName = worksheet.Cells[row, 4].GetValue<string>()?.Trim();
                        dto.ClientEmail = worksheet.Cells[row, 5].GetValue<string>()?.Trim();
                        dto.ClientAddress = worksheet.Cells[row, 6].GetValue<string>()?.Trim();
                        dto.ClientPhone = worksheet.Cells[row, 7].GetValue<string>()?.Trim();

                        dto.ProductName = worksheet.Cells[row, 8].GetValue<string>()?.Trim();
                        dto.ProductDescription = worksheet.Cells[row, 9].GetValue<string>()?.Trim();
                        dto.UnitPrice = worksheet.Cells[row, 10].GetValue<decimal>();
                        dto.Quantity = worksheet.Cells[row, 11].GetValue<int>();

                        // Validation
                        if (string.IsNullOrEmpty(dto.ClientDocument) || string.IsNullOrEmpty(dto.ProductName))
                        {
                            dto.IsValid = false;
                            dto.ErrorMessage = "Missing Client Document or Product Name.";
                        }
                    }
                    catch (Exception ex)
                    {
                        dto.IsValid = false;
                        dto.ErrorMessage = $"Error reading row: {ex.Message}";
                    }
                    
                    rows.Add(dto);
                }
            }

            return await Task.FromResult(rows);
        }
    }
