using Microsoft.AspNetCore.Mvc.RazorPages;

using Firmness.Domain.Entities;
using Firmness.Infraestructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Firmness.Web.Pages.Sales
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Sale> Sales { get; set; } = default!;

        public async Task OnGetAsync()
        {   //The page loads the sales list with the client
            Sales = await _context.Sales
                .Include(s => s.Client)
                .OrderByDescending(s => s.SaleDate) // the latest first
                .ToListAsync();
        }
    }
}