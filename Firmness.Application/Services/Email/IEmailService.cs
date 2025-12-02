namespace Firmness.Application.Services.Email;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string messageBody, byte[]? attachmentData = null, string? attachmentName = null);
}