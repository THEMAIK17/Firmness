using Firmness.Application.Services;
using OfficeOpenXml; 

namespace Firmness.Tests
{
    public class ExcelServiceTests
    {
        public ExcelServiceTests()
        {
            
            ExcelPackage.License.SetNonCommercialPersonal("maikol");
        }

        [Fact]
        [Obsolete("Obsolete")]
        public async Task ReadSalesFileAsync_WhenProductIsMissing_ShouldMarkRowAsInvalid()
        {
            
            // Create a "fake" in-memory Excel file
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Sheet1");

            // Headers (Row 1)
            worksheet.Cells[1, 1].Value = "Invoice";
            worksheet.Cells[1, 2].Value = "Date";
            worksheet.Cells[1, 3].Value = "ClientDoc";
            
            // ... other columns ...
            worksheet.Cells[1, 8].Value = "ProductName";

            // Test Data (Row 2) 
            worksheet.Cells[2, 1].Value = 1001;
            worksheet.Cells[2, 2].Value = DateTime.Now;
            worksheet.Cells[2, 3].Value = "12345"; // Client Document
           
            worksheet.Cells[2, 10].Value = 100; 
            worksheet.Cells[2, 11].Value = 1;   

            // Convert the Excel to a Stream 
            var stream = new MemoryStream(package.GetAsByteArray());
            
            var service = new ExcelService();
            
            var result = await service.ReadSalesFileAsync(stream);

            //  Assert
            Assert.NotNull(result);
            Assert.Single(result); 
            
            var row = result.First();
            Assert.False(row.IsValid); 
            Assert.Contains("Missing Client Document or Product Name", row.ErrorMessage);
        }
        
        [Fact]
        [Obsolete("Obsolete")]
        public async Task ReadSalesFileAsync_WhenDataIsValid_ShouldReturnValidRow()
        {
            //  Arrange
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Sheet1");

            // Headers
            worksheet.Cells[1, 8].Value = "ProductName";

            // Complete Data
            worksheet.Cells[2, 1].Value = 1001;
            worksheet.Cells[2, 2].Value = DateTime.Now;
            worksheet.Cells[2, 3].Value = "12345";
            worksheet.Cells[2, 8].Value = "Concrete"; 
            worksheet.Cells[2, 10].Value = 50;
            worksheet.Cells[2, 11].Value = 2;

            var stream = new MemoryStream(package.GetAsByteArray());
            var service = new ExcelService();

            // Act
            var result = await service.ReadSalesFileAsync(stream);

            //  Assert
            var row = result.First();
            Assert.True(row.IsValid);
            Assert.Equal("Concrete", row.ProductName);
        }
    }
}