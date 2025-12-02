using Firmness.Domain.Entities;
using Firmness.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Firmness.Web.Pages.Clients
{
    // This PageModel implements TASK 7 
    [NoCache]
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<Client> _userManager;

        // Property to hold the list of clients for the Razor page
        public IList<Client> ClientList { get; set; } = new List<Client>();

        // Property for the search (TASK 7)
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public IndexModel(UserManager<Client> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            
            var query = _userManager.Users;

            // Apply search filter if SearchTerm is provided 
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(u => 
                    (u.FirstName.Contains(SearchTerm)) ||
                    (u.LastName.Contains(SearchTerm)) ||
                    (u.DocumentNumber.Contains(SearchTerm))
                );
            }

            // Execute the query
            ClientList = await query.ToListAsync();
        }
    }
}