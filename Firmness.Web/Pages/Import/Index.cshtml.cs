using Firmness.Application.DTOs;
using Firmness.Application.Services;
using Firmness.Domain.Entities;
using Firmness.Infraestructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace Firmness.Web.Pages.Import
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly IExcelService _excelService;
        private readonly ApplicationDbContext _context;

        public IndexModel(IExcelService excelService, ApplicationDbContext context)
        {
            _excelService = excelService;
            _context = context;
        }

        [BindProperty]
        public IFormFile Upload { get; set; } = default!;

        public List<string> Messages { get; set; } = new List<string>();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Upload == null || Upload.Length == 0)
            {
                ModelState.AddModelError("", "Please upload a valid Excel file.");
                return Page();
            }

            //  Read the Excel file using the Application Service
            List<ExcelDataRow> rawRows;
            using (var stream = Upload.OpenReadStream())
            {
                rawRows = await _excelService.ReadSalesFileAsync(stream);
            }

            // Report reading errors
            foreach (var row in rawRows.Where(r => !r.IsValid))
            {
                Messages.Add($"Row Error: {row.ErrorMessage}");
            }
            
            var validRows = rawRows.Where(r => r.IsValid).ToList();
            if (validRows.Count == 0)
            {
                Messages.Add("No valid rows found to process.");
                return Page();
            }

            //  Normalize and Process Data 
            // I use a transaction to ensure data consistency.
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Group rows by InvoiceNumber to create unique Sales
                var salesGroups = validRows.GroupBy(r => r.InvoiceNumber);
                int salesCount = 0;

                foreach (var saleGroup in salesGroups)
                {
                    var firstRow = saleGroup.First();

                    //  Find or Create Client 
                    var client = await _context.Users.OfType<Client>()
                        .FirstOrDefaultAsync(c => c.DocumentNumber == firstRow.ClientDocument);

                    if (client == null)
                    {
                        client = new Client
                        {
                            UserName = firstRow.ClientEmail ?? $"user{Guid.NewGuid()}", 
                            Email = firstRow.ClientEmail,
                            DocumentNumber = firstRow.ClientDocument,
                            FirstName = firstRow.ClientName, 
                            LastName = ".", 
                            Address = firstRow.ClientAddress ?? "",
                            PhoneNumber = firstRow.ClientPhone,
                            EmailConfirmed = true 
                        };
                        
                        _context.Users.Add(client);
                        await _context.SaveChangesAsync(); 
                    }

                    //  Create Sale Header
                    var sale = new Sale
                    {
                        ClientId = client.Id,
                        SaleDate = firstRow.Date.ToUniversalTime(),
                        TotalAmount = 0, 
                        TaxAmount = 0
                    };
                    _context.Sales.Add(sale);
                    await _context.SaveChangesAsync(); 

                    
                    decimal totalSale = 0;
                    foreach (var item in saleGroup)
                    {
                        // Find or Create Product 
                        var product = await _context.Products
                            .FirstOrDefaultAsync(p => p.Name == item.ProductName);

                        if (product == null)
                        {
                            product = new Product
                            {
                                Name = item.ProductName,
                                Description = item.ProductDescription ?? item.ProductName,
                                UnitPrice = item.UnitPrice,
                                Stock = 100 // Default stock for imported items
                            };
                            _context.Products.Add(product);
                            await _context.SaveChangesAsync();
                        }

                        // Create Sale Detail
                        var detail = new SaleDetail
                        {
                            SaleId = sale.Id,
                            ProductId = product.Id,
                            Quantity = item.Quantity,
                            UnitPriceAtSale = item.UnitPrice
                        };
                        _context.SaleDetails.Add(detail);

                        totalSale += (item.Quantity * item.UnitPrice);
                    }

                    // Update Sale Totals
                    sale.TotalAmount = totalSale;
                    sale.TaxAmount = totalSale * 0.19m; 
                    _context.Sales.Update(sale);
                    
                    salesCount++;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                Messages.Add($"Success: {salesCount} sales imported and normalized.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Messages.Add($"Critical Database Error: {ex.Message}");
            }

            return Page();
        }
    }
}