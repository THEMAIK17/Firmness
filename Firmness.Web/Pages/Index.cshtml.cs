
using Firmness.Infraestructure.Data;
using Firmness.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Firmness.Web.Pages
{
    [Authorize(Roles = "Administrator")]
    [NoCache]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        
 
        public int TotalProducts { get; set; }
        public int TotalClients { get; set; }
        public int TotalSales { get; set; }

     
        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

 
        public async Task OnGetAsync()
        {
         
            TotalProducts = await _context.Products.CountAsync();
            
            
            TotalClients = await _context.Users.CountAsync(); 
            
            TotalSales = await _context.Sales.CountAsync();
        }
    }
}