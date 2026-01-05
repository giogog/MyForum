using Contracts;
using Domain.Exception;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Domain.Models;

namespace Infrastructure.Email;

public class EmailSender : IEmailSender
{
    private readonly EmailOptions _emailSettings;
    public EmailSender(IOptions<EmailOptions> emailSettings)
    {
        _emailSettings = emailSettings.Value;

    }
    public async Task<EmailResult> SendEmailAsync(string email, string subject, string htmlMessage)
    {
        try
        {
            using var client = new SmtpClient(_emailSettings.MailServer, _emailSettings.MailPort)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.FromEmail, _emailSettings.Password),
                Timeout = 30000 // 30 second timeout
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
                Priority = MailPriority.Normal
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage).ConfigureAwait(false);

            return new EmailResult { IsSuccess = true };
        }
        catch (SmtpException smtpEx)
        {
            return new EmailResult { IsSuccess = false, ErrorMessage = $"SMTP Error: {smtpEx.Message}" };
        }
        catch (Exception ex)
        {
            return new EmailResult { IsSuccess = false, ErrorMessage = $"Email sending failed: {ex.Message}" };
        }
    }
}
