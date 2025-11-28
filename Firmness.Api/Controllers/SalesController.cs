using AutoMapper;
using Firmness.Api.DTOs.Sales;
using Firmness.Application.Services;
using Firmness.Domain.Entities;
using Firmness.Infraestructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Firmness.Application.Services.Email;

namespace Firmness.Api.Controllers;

[Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService; 
        private readonly IPdfService _pdfService;     // Service of the PDF

        public SalesController(ApplicationDbContext context, IMapper mapper, IEmailService emailService, IPdfService pdfService)
        {
            _context = context;
            _mapper = mapper;
            _emailService = emailService;
            _pdfService = pdfService;
        }

        // GET: api/Sales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetSales()
        {
            var sales = await _context.Sales
                .Include(s => s.Client)      
                .Include(s => s.SaleDetails) 
                    .ThenInclude(sd => sd.Product) 
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<SaleDto>>(sales));
        }

        // GET: api/Sales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SaleDto>> GetSale(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Client)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null) return NotFound();

            return Ok(_mapper.Map<SaleDto>(sale));
        }

        // POST: api/Sales
        [HttpPost]
        public async Task<ActionResult<SaleDto>> CreateSale(CreateSaleDto createDto)
        {
            if (createDto.Items == null || !createDto.Items.Any())
            {
                return BadRequest("A sale must contain at least one product.");
            }

            // I use one transaction for integrity constraints
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
               
                var sale = new Sale
                {
                    ClientId = createDto.ClientId,
                    SaleDate = DateTime.UtcNow,
                    TaxAmount = 0,
                    TotalAmount = 0
                };

                decimal total = 0;

              
                foreach (var itemDto in createDto.Items)
                {
                    var product = await _context.Products.FindAsync(itemDto.ProductId);
                    
                    if (product == null) 
                        throw new Exception($"Product ID {itemDto.ProductId} not found.");
                    
                    if (product.Stock < itemDto.Quantity) 
                        throw new Exception($"Insufficient stock for product '{product.Name}'.");
                    
                    product.Stock -= itemDto.Quantity;
                    
                    var detail = new SaleDetail
                    {
                        Sale = sale,
                        ProductId = product.Id,
                        Quantity = itemDto.Quantity,
                        UnitPriceAtSale = product.UnitPrice 
                    };

                    total += (detail.Quantity * detail.UnitPriceAtSale);
                    _context.SaleDetails.Add(detail);
                }

                
                sale.TaxAmount = total * 0.19m; 
                sale.TotalAmount = total + sale.TaxAmount;

                _context.Sales.Add(sale);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                // generate pdf
                var saleForPdf = await _context.Sales
                    .Include(s => s.Client)
                    .Include(s => s.SaleDetails).ThenInclude(sd => sd.Product)
                    .FirstAsync(s => s.Id == sale.Id);
                
                var pdfBytes = await _pdfService.GenerateReceiptAsync(saleForPdf);

                
                string emailBody = $@"
                <h1>¡Gracias por tu compra!</h1>
                <p>Adjunto encontrarás el recibo de tu compra #{sale.Id}.</p>
                <p><strong>Total:</strong> ${sale.TotalAmount:N2}</p>";

              
                try 
                {
                    await _emailService.SendEmailAsync(
                        saleForPdf.Client.Email!, 
                        $"Recibo de Compra #{sale.Id}", 
                        emailBody,
                        pdfBytes,                      
                        $"Recibo_{sale.Id}.pdf"       
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error enviando correo: {ex.Message}");
                    
                }
                return CreatedAtAction("GetSale", new { id = sale.Id }, new { id = sale.Id, message = "Sale created successfully" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }
    }