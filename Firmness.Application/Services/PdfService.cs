// Location: Firmeza.Application/Services/PdfService.cs
using Firmness.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Threading.Tasks;
namespace Firmness.Application.Services;
public class PdfSerivce
{
    // This implements the PDF generation 
    public class PdfService : IPdfService
    {
        public PdfService()
        {
            // Set QuestPDF license (Community is free)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GenerateReceiptAsync(Sale sale)
        {
            // Use Task.Run for CPU-bound generation
            return await Task.Run(() =>
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.Letter);
                        page.Margin(30);
                        page.DefaultTextStyle(style => style.FontSize(12));

                        // Header
                        page.Header().Row(row =>
                        {
                            row.RelativeItem().Text("FIRMEZA S.A.S.")
                                .Bold().FontSize(20);
                            
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text($"Receipt #: {sale.Id}").Bold();
                                col.Item().Text($"Sale Date: {sale.SaleDate:yyyy-MM-dd}");
                            });
                        });

                        // Content (Client Info)
                        page.Content().Column(col =>
                        {
                            col.Item().PaddingTop(20).Text("Client Information").Bold().FontSize(14);
                            col.Item().Text($"Client: {sale.Client.FullName}"); // Using the NotMapped FullName
                            col.Item().Text($"Email: {sale.Client.Email}");
                            col.Item().Text($"Document: {sale.Client.DocumentNumber}");

                            // Table of Products (Sale Details)
                            col.Item().PaddingTop(20).Text("Sale Details").Bold().FontSize(14);
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3); // Product Name
                                    columns.RelativeColumn(1); // Quantity
                                    columns.RelativeColumn(1); // Unit Price
                                    columns.RelativeColumn(1); // Total
                                });
                                
                                table.Header(header =>
                                {
                                    header.Cell().Text("Product");
                                    header.Cell().Text("Quantity").AlignCenter();
                                    header.Cell().Text("Unit Price").AlignRight();
                                    header.Cell().Text("Total").AlignRight();
                                });
                                
                                foreach (var item in sale.SaleDetails)
                                {
                                    // Assumes Product is loaded (which it should be)
                                    table.Cell().Text(item.Product?.Name ?? "Product Error");
                                    table.Cell().Text(item.Quantity).AlignCenter();
                                    table.Cell().Text($"${item.UnitPriceAtSale:N2}").AlignRight();
                                    table.Cell().Text($"${(item.Quantity * item.UnitPriceAtSale):N2}").AlignRight();
                                }
                                
                                // Totals
                                table.Cell().ColumnSpan(3).Text("Subtotal").AlignRight().Bold();
                                table.Cell().Text($"${(sale.TotalAmount - sale.TaxAmount):N2}").AlignRight().Bold();
                                
                                table.Cell().ColumnSpan(3).Text("Tax (IVA 19%)").AlignRight().Bold();
                                table.Cell().Text($"${sale.TaxAmount:N2}").AlignRight().Bold();
                                
                                table.Cell().ColumnSpan(3).Text("TOTAL").AlignRight().Bold().FontSize(14);
                                table.Cell().Text($"${sale.TotalAmount:N2}").AlignRight().Bold().FontSize(14);
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

                // Generate PDF as byte array
                return document.GeneratePdf();
            });
        }
    }
}
