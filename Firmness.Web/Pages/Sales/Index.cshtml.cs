using Firmness.Application.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Firmness.Domain.Entities;
using Firmness.Infraestructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firmness.Web.Pages.Sales
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IPdfService _pdfService;         
        private readonly IWebHostEnvironment _hostEnvironment;

        public IndexModel(ApplicationDbContext context, IPdfService pdfService, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _pdfService = pdfService;
            _hostEnvironment = hostEnvironment;
        }

        public IList<Sale> Sales { get; set; } = default!;

        public async Task OnGetAsync()
        {   //The page loads the sales list with the client
            Sales = await _context.Sales
                .Include(s => s.Client)
                .OrderByDescending(s => s.SaleDate) // the latest first
                .ToListAsync();
        }
        // On-Demand PDF Generation
        public async Task<IActionResult> OnGetDownloadPdfAsync(int id)
        {
            string webRootPath = _hostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string receiptsPath = Path.Combine(webRootPath, "recibos");
            string fileName = $"Receipt_Sale_{id}.pdf";
            string filePath = Path.Combine(receiptsPath, fileName);

            byte[] pdfBytes; 

            if (System.IO.File.Exists(filePath))
            {
                pdfBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            }
            else
            {
                var sale = await _context.Sales
                    .Include(s => s.Client)
                    .Include(s => s.SaleDetails).ThenInclude(sd => sd.Product)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (sale == null) return NotFound();
                
                pdfBytes = await _pdfService.GenerateReceiptAsync(sale);
                
                try
                {
                    if (!Directory.Exists(receiptsPath)) Directory.CreateDirectory(receiptsPath);
                    await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" Alerta: No se pudo guardar el archivo físico (posible error de permisos), pero se enviará al usuario. Error: {ex.Message}");
                }
                // -----------------------------------------------------
            }
            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}