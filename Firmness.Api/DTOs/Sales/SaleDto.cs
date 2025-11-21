namespace Firmness.Api.DTOs.Sales;

public class SaleDto
{
    public int Id { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
        
    public string ClientId { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty; 
        
    public List<SaleDetailDto> SaleDetails { get; set; } = new List<SaleDetailDto>();
}

public class SaleDetailDto
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPriceAtSale { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty; 
}

public class CreateSaleDto
{
    public string ClientId { get; set; } = string.Empty;
    public List<CreateSaleDetailDto> Items { get; set; } = new List<CreateSaleDetailDto>();
}

public class CreateSaleDetailDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}