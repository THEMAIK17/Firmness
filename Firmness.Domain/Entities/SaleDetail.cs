namespace Firmness.Domain.Entities;

public class SaleDetail
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPriceAtSale { get; set; } // Price lock for history/receipts
        
    // Foreign Keys
    public int SaleId { get; set; }
    public Sale Sale { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}