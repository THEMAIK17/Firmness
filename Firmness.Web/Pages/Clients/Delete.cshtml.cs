using Firmness.Domain.Entities; // Your Client entity
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Firmness.Web.Pages.Clients
{
    // This PageModel implements the Delete logic for TASK 7
    public class DeleteModel : PageModel
    {
        private readonly UserManager<Client> _userManager;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(UserManager<Client> userManager, ILogger<DeleteModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        // We bind this property to display the client's info
        [BindProperty]
        public Client Client { get; set; } = default!;
        
        public string? ErrorMessage { get; set; }

        // OnGetAsync runs when the page loads, fetching the client to be deleted
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _userManager.FindByIdAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            
            Client = client;
            return Page();
        }

        // OnPostAsync runs when the user confirms the deletion
        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _userManager.FindByIdAsync(id);
            if (client == null)
            {
                // Client already deleted or doesn't exist
                return RedirectToPage("./Index");
            }
            
            // Prevent the Admin from deleting their own account
            if (client.Email == "admin@firmeza.com")
            {
                ErrorMessage = "Error: Cannot delete the default Administrator account.";
                Client = client; // Re-bind client data to display the page again
                return Page();
            }

            // Use UserManager to delete the client
            var result = await _userManager.DeleteAsync(client);

            if (result.Succeeded)
            {
                _logger.LogInformation($"Client with ID '{client.Id}' deleted.");
                return RedirectToPage("./Index");
            }

            // If deletion fails (e.g., database error)
            ErrorMessage = "Error deleting client.";
            foreach (var error in result.Errors)
            {
                ErrorMessage += $" {error.Description}";
            }
            
            Client = client; // Re-bind client data
            return Page();
        }
    }
}