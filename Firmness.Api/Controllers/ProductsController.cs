using AutoMapper;
using Firmness.Api.DTOs.Products;
using Firmness.Domain.Entities;
using Firmness.Infraestructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firmness.Api.Controllers;

  [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper; // dependency new

        // Inyectamos Mapper además del Contexto
        public ProductsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            // take data of the  BD
            var products = await _context.Products.ToListAsync();
            
            //  Change to DTOs using AutoMapper 
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            
            return Ok(productDtos);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

           
            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, CreateProductDto productDto)
        {
           
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            
            _mapper.Map(productDto, product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<ProductDto>> PostProduct(CreateProductDto productDto)
        {
           
            var product = _mapper.Map<Product>(productDto);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            
            var resultDto = _mapper.Map<ProductDto>(product);

            // Retorna 201 Created
            return CreatedAtAction("GetProduct", new { id = product.Id }, resultDto);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }  
