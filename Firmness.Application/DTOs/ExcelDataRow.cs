using System;
namespace Firmness.Application.DTOs;

/// Represents a single row from the denormalized Excel file.
public class ExcelDataRow
{
    // Sale Information 
    public int InvoiceNumber { get; set; } 
    public DateTime Date { get; set; }

    // Client Information (Denormalized)
    public string? ClientDocument { get; set; } = string.Empty;
    public string? ClientName { get; set; } = string.Empty;
    public string? ClientEmail { get; set; } = string.Empty;
    public string? ClientAddress { get; set; } = string.Empty;
    public string? ClientPhone { get; set; } = string.Empty;

    // Product Information (Denormalized) 
    public string? ProductName { get; set; } = string.Empty;
    public string? ProductDescription { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
        
    // Validation Status 
    public bool IsValid { get; set; } = true;
    public string ErrorMessage { get; set; } = string.Empty;
}

    