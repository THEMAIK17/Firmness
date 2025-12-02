
using Firmness.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Firmness.Application.Services;

    // This implements the PDF generation 
  public class PdfService : IPdfService
{
    public PdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateReceiptAsync(Sale sale)
    {
        return await Task.Run(() =>
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(30);
                    page.DefaultTextStyle(style => style.FontSize(12));

                    //  Header
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Text("FIRMEZA S.A.S.").Bold().FontSize(20);
                        
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text($"Receipt #: {sale.Id}").Bold();
                            col.Item().Text($"Sale Date: {sale.SaleDate:yyyy-MM-dd}");
                        });
                    });

                    // Content (Client Info & Table)
                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Item().Text("Client Information").Bold().FontSize(14);
                        col.Item().Text($"Client: {sale.Client.FullName}");
                        col.Item().Text($"Email: {sale.Client.Email}");
                        col.Item().Text($"Document: {sale.Client.DocumentNumber}");

                        // Table
                        col.Item().PaddingTop(10).Text("Sale Details").Bold().FontSize(14);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3); 
                                columns.RelativeColumn(1); 
                                columns.RelativeColumn(1); 
                                columns.RelativeColumn(1); 
                            });
                            
                            table.Header(header =>
                            {
                                header.Cell().Text("Product");
                                header.Cell().AlignCenter().Text("Quantity");
                                header.Cell().AlignRight().Text("Unit Price");
                                header.Cell().AlignRight().Text("Total");
                            });
                            
                            foreach (var item in sale.SaleDetails)
                            {
                                table.Cell().Text(item.Product?.Name ?? "Product Error");
                                table.Cell().AlignCenter().Text(item.Quantity.ToString());
                                table.Cell().AlignRight().Text($"${item.UnitPriceAtSale:N2}");
                                table.Cell().AlignRight().Text($"${(item.Quantity * item.UnitPriceAtSale):N2}");
                            }
                            
                            // Totals
                            table.Cell().ColumnSpan(3).AlignRight().Text("Subtotal").Bold();
                            table.Cell().AlignRight().Text($"${(sale.TotalAmount - sale.TaxAmount):N2}").Bold();
                            
                            table.Cell().ColumnSpan(3).AlignRight().Text("Tax (IVA 19%)").Bold();
                            table.Cell().AlignRight().Text($"${sale.TaxAmount:N2}").Bold();
                            
                            table.Cell().ColumnSpan(3).AlignRight().Text("TOTAL").Bold().FontSize(14);
                            table.Cell().AlignRight().Text($"${sale.TotalAmount:N2}").Bold().FontSize(14);
                        });
                    }); 
                    
                    // Footer
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                    });
                });
            });

            return document.GeneratePdf();
        });
    }
}