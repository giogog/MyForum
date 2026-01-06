using Domain.Exception;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace Contracts;

public interface IEmailService
{
    
    Task<IdentityResult> ConfirmEmailAsync(string userId, string token);
    Task<EmailResult> SendConfirmationMail(IUrlHelper urlHelper, string username);
    Task<EmailResult> CheckMailConfirmation(string Username);
    Task<EmailResult> SendPasswordResetEmail(IUrlHelper urlHelper, string email);
}
