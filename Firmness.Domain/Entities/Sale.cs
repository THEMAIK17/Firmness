namespace Firmness.Domain.Entities;

public class Sale
{
    public int Id { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; } // Required for IVA calculation
        
    // Foreign Key to Client/IdentityUser
    public string ClientId { get; set; } = string.Empty; 
    public Client Client { get; set; } = null!; // Navigation property

    // Navigation property for Sale Details 
    public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
}