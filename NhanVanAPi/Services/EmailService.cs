using SendGrid;
using SendGrid.Helpers.Mail;

namespace NhanVanAPi.Services;

public class EmailService
{
    private readonly IConfiguration _config;//it allows your application to read configuration values.

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendConfirmationEmailAsync(string toEmail, string confirmationLink)
    {
        var apiKey = _config["SendGrid:ApiKey"];
        var fromEmail = _config["SendGrid:FromEmail"];
        var fromName = _config["SendGrid:FromName"];

        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(fromEmail, fromName);
        var to = new EmailAddress(toEmail);
        var subject = "Please Confirm your email - NhanVan IELTS Platform";
        var htmlContent = $"<p>Please confirm your email by clicking the link below:</p>" +
                           $"<p><a href=\"{confirmationLink}\">Confirm Email</a></p>";

        var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
        await client.SendEmailAsync(msg);
    }
}
