using Firmness.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Firmness.Web.Pages.Clients
{
    // This PageModel implements the Update logic for TASK 7
    public class EditModel : PageModel
    {
        private readonly UserManager<Client> _userManager;

        public EditModel(UserManager<Client> userManager)
        {
            _userManager = userManager;
        }

        // We bind the InputModel to receive data from the form
        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        
        public string? Email { get; set; } // To display the email (read-only)

        // The InputModel defines the fields we can edit
        public class InputModel
        {
            [Required]
            public string Id { get; set; } = string.Empty;
            [Required]
            public string FirstName { get; set; } = string.Empty;
            [Required]
            public string LastName { get; set; } = string.Empty;
            [Required]
            public string DocumentNumber { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
        }

        // OnGetAsync runs when the page loads, fetching the client data
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
            
            // Load data into the InputModel
            Input = new InputModel
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                DocumentNumber = client.DocumentNumber,
                Address = client.Address,
                PhoneNumber = client.PhoneNumber ?? ""
            };
            
            Email = client.Email; // Set the read-only email

            return Page();
        }

        // OnPostAsync runs when the form is submitted
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = await _userManager.FindByIdAsync(Input.Id);
            if (client == null)
            {
                return NotFound();
            }

            // Update the custom properties
            client.FirstName = Input.FirstName;
            client.LastName = Input.LastName;
            client.DocumentNumber = Input.DocumentNumber;
            client.Address = Input.Address;
            client.PhoneNumber = Input.PhoneNumber;

            // Use UserManager to save the changes
            var result = await _userManager.UpdateAsync(client);

            if (result.Succeeded)
            {
                // Redirect back to the list page
                return RedirectToPage("./Index"); 
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
    }
}