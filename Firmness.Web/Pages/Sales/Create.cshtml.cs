
using Firmness.Domain.Entities;
using Firmness.Infraestructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering; 
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Firmness.Application.Services;

namespace Firmness.Web.Pages.Sales
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IPdfService _pdfService; 
        private readonly IWebHostEnvironment _hostEnvironment;
        
        public CreateModel(ApplicationDbContext context, IPdfService pdfService, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _pdfService = pdfService;
            _hostEnvironment = hostEnvironment;
        }

        // --- Properties to populate the Form Dropdowns ---
        public SelectList ClientList { get; set; } = default!;
        public SelectList ProductList { get; set; } = default!;
        
        // I will store the JSON representation of product prices for JavaScript
        public string ProductPricesJson { get; set; } = "{}";

       
        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        
        public class InputModel
        {
            [Required]
            [Display(Name = "Client")]
            public string ClientId { get; set; } = string.Empty;

            // This will capture the list of products added via JavaScript
            public List<SaleDetailInput> Items { get; set; } = new List<SaleDetailInput>();
        }

        public class SaleDetailInput
        {
            [Required]
            public int ProductId { get; set; }
            [Required]
            [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
            public int Quantity { get; set; }
            [Required]
            public decimal UnitPrice { get; set; } 
            public string ProductName { get; set; } = string.Empty;
        }

        // OnGet loads the dropdown menus
        public async Task OnGetAsync()
        {
            // Load Clients (Users) for the dropdown
            ClientList = new SelectList(await _context.Users.ToListAsync(), "Id", "FullName");
            
            // Load Products with stock for the dropdown
            var availableProducts = await _context.Products
                .Where(p => p.Stock > 0)
                .Select(p => new { p.Id, p.Name, p.UnitPrice })
                .ToListAsync();
                
            ProductList = new SelectList(availableProducts, "Id", "Name");
            
            // Create a dictionary for JS to know the prices
            var productPrices = availableProducts.ToDictionary(p => p.Id, p => p.UnitPrice);
            ProductPricesJson = System.Text.Json.JsonSerializer.Serialize(productPrices);
        }

        // OnPost saves the Sale using a Transaction
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || Input.Items.Count == 0)
            {
                await OnGetAsync(); 
                if (Input.Items.Count == 0)
                    ModelState.AddModelError(string.Empty, "You must add at least one product to the sale.");
                return Page();
            }
            
            // This ensures that the Sale AND SaleDetails are saved, or neither are.
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                //  Create the master Sale entity
                var sale = new Sale
                {
                    ClientId = Input.ClientId,
                    SaleDate = DateTime.UtcNow,
                    TaxAmount = 0, // We calculate this next
                    TotalAmount = 0 // We calculate this next
                };

                //  Create SaleDetails from the input and calculate totals
                decimal total = 0;
                foreach (var item in Input.Items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product == null || product.Stock < item.Quantity)
                    {
                        throw new Exception($"Product '{item.ProductName}' is out of stock or does not exist.");
                    }

                    // Reduce stock
                    product.Stock -= item.Quantity;

                    var saleDetail = new SaleDetail
                    {
                        Sale = sale, 
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPriceAtSale = item.UnitPrice
                    };
                    
                    total += (item.UnitPrice * item.Quantity);
                    _context.SaleDetails.Add(saleDetail);
                }

                //  Set totals (Assuming 19% IVA for example, adjust as needed)
                sale.TaxAmount = total * 0.19m;
                sale.TotalAmount = total + sale.TaxAmount;
                _context.Sales.Add(sale);
               
                await _context.SaveChangesAsync();
                
                await transaction.CommitAsync();
                
                var saleForPdf = await _context.Sales
                    .Include(s => s.Client) 
                    .Include(s => s.SaleDetails) 
                    .ThenInclude(sd => sd.Product) 
                    .FirstOrDefaultAsync(s => s.Id == sale.Id);

                if (saleForPdf != null)
                {
                   
                    byte[] pdfBytes = await _pdfService.GenerateReceiptAsync(saleForPdf);
                    
                   
                    string receiptsFolderPath = Path.Combine(_hostEnvironment.WebRootPath, "recibos");
                    if (!Directory.Exists(receiptsFolderPath))
                    {
                        Directory.CreateDirectory(receiptsFolderPath);
                    }
                    string fileName = $"Receipt_Sale_{saleForPdf.Id}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                    string filePath = Path.Combine(receiptsFolderPath, fileName);
                    
                    await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);
                }
                
                return RedirectToPage("/Index"); 
            }
            catch (Exception ex)
            {
                // If anything fails, roll back the entire transaction
                await transaction.RollbackAsync();
                await OnGetAsync(); 
                ModelState.AddModelError(string.Empty, $"Error creating sale: {ex.Message}");
                return Page();
            }
        }
    }
}