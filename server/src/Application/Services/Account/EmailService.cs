using Contracts;
using Domain.Exception;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services;
public class EmailService : IEmailService
{
    private readonly IEmailSender _emailSender;
    private readonly IRepositoryManager _repositoryManager;
    private readonly ITokenGenerator _tokenGenerator;
    public EmailService(
        IEmailSender emailSender,
        IRepositoryManager repoManager,
        ITokenGenerator tokenGenerator
        )
    {

        _emailSender = emailSender;
        _repositoryManager = repoManager;
        _tokenGenerator = tokenGenerator;
    }
    public async Task<EmailResult> SendConfirmationMail(IUrlHelper urlHelper, string username)
    {
        // Retrieve the user from the repository
        var user = await _repositoryManager.UserRepository.GetUser(user => user.UserName == username);
        if (user == null)
        {
            // Handle the case where the user is not found
            return new EmailResult { IsSuccess = false, ErrorMessage = "User not found." };
        }

        // Generate the email confirmation token
        var token = await _tokenGenerator.GenerateMailTokenCode(user);
        if (string.IsNullOrEmpty(token))
        {
            // Handle the case where the token generation fails
            return new EmailResult { IsSuccess = false, ErrorMessage = "Token generation failed." };
        }

        // Generate the callback URL
        var callbackUrl = urlHelper.Action(
            action: "ConfirmEmail",
            controller: "Account",
            values: new { userId = user.Id, token },
            protocol: "https");

        if (string.IsNullOrEmpty(callbackUrl))
        {
            // Handle the case where the URL generation fails
            return new EmailResult { IsSuccess = false, ErrorMessage = "Callback URL generation failed." };
        }

        // Send the confirmation email with HTML encoding for security
        var htmlMessage = $"<p>Please confirm your account by <a href='{System.Net.WebUtility.HtmlEncode(callbackUrl)}'>clicking here</a>.</p>" +
            $"<p>If you did not create an account, please ignore this email.</p>";
        
        return await _emailSender.SendEmailAsync(user.Email!, "Confirm your email", htmlMessage);
    }


    public async Task<EmailResult> CheckMailConfirmation(string Username)
    {
        var user = await _repositoryManager.UserRepository.GetUser(u => u.UserName == Username);

        if (!user.EmailConfirmed)
            return new EmailResult { IsSuccess = false };

        return new EmailResult { IsSuccess = true, ErrorMessage = "Mail is Confirmed" };


    }

    public async Task<EmailResult> SendPasswordResetEmail(IUrlHelper urlHelper,string email)
    {
        var user = await _repositoryManager.UserRepository.GetUser(u => u.Email == email);
        if (user == null)
            throw new NotFoundException("User not found");
        if (!user.EmailConfirmed)
            throw new MailNotConfirmedException("Email is not confirmed");

        string token =  await _tokenGenerator.GeneratePasswordResetToken(user);
        var callbackUrl = urlHelper.Action(
            action: "ResetPasswordPage",
            controller: "Account",
            values: new { token, email },
            protocol: "https");


        if (string.IsNullOrEmpty(callbackUrl))
        {
            // Handle the case where the URL generation fails
            return new EmailResult { IsSuccess = false, ErrorMessage = "Callback URL generation failed." };
        }

        var htmlMessage = $"<p>You requested a password reset. Please reset your password by <a href='{System.Net.WebUtility.HtmlEncode(callbackUrl)}'>clicking here</a>.</p>" +
            $"<p>This link will expire in 24 hours.</p>" +
            $"<p>If you did not request a password reset, please ignore this email and your password will remain unchanged.</p>";
        
        return await _emailSender.SendEmailAsync(email, "Reset Password", htmlMessage);
    }

    // Method to confirm the user's email
    public async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
    {
        var user = await _repositoryManager.UserRepository.GetUser(u => u.Id == Int32.Parse(userId));
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Code = "UserNotFound", Description = "User not found." });
        }
        return await _repositoryManager.UserRepository.ConfirmEmail(user, token);
    }
}
