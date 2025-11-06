namespace Firmness.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    // Required for Excel import validation (price, quantity)
    public decimal UnitPrice { get; set; } 
    public int Stock { get; set; }

    // Navigation property for Sale Details 
    public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
}