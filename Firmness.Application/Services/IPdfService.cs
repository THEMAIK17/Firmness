using Firmness.Domain.Entities;

namespace Firmness.Application.Services;

public interface IPdfService
{    // This method generates the PDF receipt for the given sale
    Task<byte[]> GenerateReceiptAsync(Sale sale);
}