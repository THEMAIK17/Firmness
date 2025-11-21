using System.Net;
using System.Net.Mail;
using Firmness.Application.Settings;
using Microsoft.Extensions.Options;

namespace Firmness.Application.Services.Email;

public class GmailEmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public GmailEmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string messageBody)
    {
        // configure the smtp client 
        var smtpClient = new SmtpClient(_settings.Host)
        {
            Port = _settings.Port,
            Credentials = new NetworkCredential(_settings.From, _settings.Password),
            EnableSsl = true,
        };

        // create the message
        var mailMessage = new MailMessage
        {
            
            From = new MailAddress(_settings.From, "Firmeza Notifications"),
            Subject = subject,
            Body = messageBody,
            IsBodyHtml = true,
        };

        mailMessage.To.Add(toEmail);

        //send the message
        await smtpClient.SendMailAsync(mailMessage);
    }
}