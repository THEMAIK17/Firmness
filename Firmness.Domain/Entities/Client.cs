using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Firmness.Domain.Entities;

public class Client: IdentityUser
{
    public string? DocumentNumber { get; set; } = string.Empty; 
    public string? FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
        
    public string Address { get; set; } = string.Empty;

   
    /// Read-only calculated property for convenience 
 
    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    // Navigation property for Sales 
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}