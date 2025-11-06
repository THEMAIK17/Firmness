using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Firmness.Domain.Entities;

public class Client: IdentityUser
{
    public string DocumentNumber { get; set; } = string.Empty; 
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
        
    public string Address { get; set; } = string.Empty;

    // --- Helper Property (Not Mapped to DB) ---
    /// Read-only calculated property for convenience (e.g., in Razor Pages UI).
    /// This field is NOT stored in the database.
 
    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    // Navigation property for Sales (1:N relationship)
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}