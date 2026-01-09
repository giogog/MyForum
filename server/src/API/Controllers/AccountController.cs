using Contracts;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers;

/// <summary>
/// Handles user authentication and account management
/// </summary>
public class AccountController(IServiceManager _serviceManager) : ApiController(_serviceManager)
{
    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <param name="registerDto">Registration information</param>
    /// <returns>Registration status and confirmation email notification</returns>
    /// <response code="201">User registered successfully, confirmation email sent</response>
    /// <response code="400">Invalid registration data or user already exists</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> Register(RegisterDto registerDto)
    {
        var registrationCheckUp = await _serviceManager.AuthorizationService.Register(registerDto);

        if (!registrationCheckUp.Succeeded)
        {
            _response = new ApiResponse(registrationCheckUp.Errors.First().Description, false, null, Convert.ToInt32(HttpStatusCode.BadRequest));
            return StatusCode(_response.StatusCode, _response);
        }


        var emailSent = await _serviceManager.EmailService.SendConfirmationMail(Url, registerDto.Username);

        if (!emailSent.IsSuccess){
            _response = new ApiResponse(emailSent.ErrorMessage ?? "Failed to send confirmation email", false, null, Convert.ToInt32(HttpStatusCode.BadRequest));
            return StatusCode(_response.StatusCode, _response);
        }
        _response = new ApiResponse("Registration successful. Please check your email to confirm your account.", true, null, Convert.ToInt32(HttpStatusCode.Created));
        return StatusCode(_response.StatusCode, _response);
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="loginDto">Login credentials containing email and password.</param>
    /// <returns>A JWT token and user information if authentication is successful.</returns>
    /// <response code="200">Returns the JWT token and user details.</response>
    /// <response code="400">If the credentials are invalid or email is not confirmed.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> Login(LoginDto loginDto)
    {
        var loginCheckUp = await _serviceManager.AuthorizationService.Login(loginDto);
        
        if (!loginCheckUp.Succeeded)
        {
            _response = new ApiResponse(loginCheckUp.Errors.First().Description, false, null, Convert.ToInt32(HttpStatusCode.BadRequest));
            return StatusCode(_response.StatusCode, _response);
        }

        _response = new ApiResponse("User logged in successfully", 
            true, 
            await _serviceManager.AuthorizationService.Authenticate(user => user.UserName == loginDto.Username), 
            Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }

    /// <summary>
    /// Confirms a user's email address using a confirmation token.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="token">The email confirmation token.</param>
    /// <returns>Redirects to login page if successful, or to error page if failed.</returns>
    /// <response code="302">Redirects to appropriate page based on confirmation result.</response>
    /// <response code="400">If the token is invalid or expired.</response>
    [HttpGet("confirm-email")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ConfirmEmail(int userId, string token)
    {
        var result = await _serviceManager.EmailService.ConfirmEmailAsync(userId.ToString(), token);

        if (!result.Succeeded)
        {
            return Redirect($"https://localhost:5003/confirmation?errormassage=Email confirmation Failed");
        }
        return Redirect("https://localhost:5003/login");

    }

    /// <summary>
    /// Resend email confirmation link to user
    /// </summary>
    /// <param name="username">Username of the account</param>
    /// <returns>Confirmation email resend status</returns>
    /// <response code="200">Confirmation email sent successfully</response>
    /// <response code="400">Email already confirmed or error sending email</response>
    [HttpPost("resend-confirmation/{username}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> ResendConfirmation(string username)
    {
        var alreadyConfirmed = await _serviceManager.EmailService.CheckMailConfirmation(username);

        if (alreadyConfirmed.IsSuccess)
        {
            _response = new ApiResponse(alreadyConfirmed.ErrorMessage ?? "Email already confirmed", false, null, Convert.ToInt32(HttpStatusCode.BadRequest));
            return StatusCode(_response.StatusCode, _response);
        }

        var emailSent = await _serviceManager.EmailService.SendConfirmationMail(Url, username);

        if (!emailSent.IsSuccess)
        {
            _response = new ApiResponse(emailSent.ErrorMessage ?? "Failed to send confirmation email", false, null, Convert.ToInt32(HttpStatusCode.BadRequest));
            return StatusCode(_response.StatusCode, _response);
        }

        _response = new ApiResponse("Please check your email to confirm your account.", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }

    /// <summary>
    /// Request password reset link
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <returns>Password reset email status</returns>
    /// <response code="201">Password reset email sent successfully</response>
    /// <response code="400">Invalid email or error sending email</response>
    [HttpPost("request-password-reset/{email}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> RequestPasswordReset(string email)
    {
        var emailSent = await _serviceManager.EmailService.SendPasswordResetEmail(Url, email);

        if (!emailSent.IsSuccess)
        {
            _response = new ApiResponse(emailSent.ErrorMessage ?? "Failed to send password reset email", false, null, Convert.ToInt32(HttpStatusCode.BadRequest));
            return StatusCode(_response.StatusCode, _response);
        }
        _response = new ApiResponse("Please check your email to for password reset.", true, null, Convert.ToInt32(HttpStatusCode.Created));
        return StatusCode(_response.StatusCode, _response);
    }

    /// <summary>
    /// Redirect to password reset page with token
    /// </summary>
    /// <param name="token">Password reset token</param>
    /// <param name="email">User's email address</param>
    /// <returns>Redirect to password reset page</returns>
    /// <response code="302">Redirects to password reset page</response>
    [HttpGet("reset-password-page")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    public async Task<IActionResult> ResetPasswordPage(string token, string email)
    {
        var encodedToken = Uri.EscapeDataString(token);
        return Redirect($"https://localhost:5003/reset-password?token={encodedToken}&email={Uri.EscapeDataString(email)}");
    }

    /// <summary>
    /// Reset user password with token
    /// </summary>
    /// <param name="resetPasswordDto">Password reset information</param>
    /// <returns>Password reset status</returns>
    /// <response code="200">Password reset successfully</response>
    /// <response code="400">Invalid token or password reset failed</response>
    [HttpPut("reset-password")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        var result = await _serviceManager.AuthorizationService.ResetPassword(resetPasswordDto);
        if (!result.Succeeded)
        {
            _response = new ApiResponse(result.Errors.First().Description, false, null, Convert.ToInt32(HttpStatusCode.BadRequest));
            return StatusCode(_response.StatusCode, _response);
        }
        _response = new ApiResponse("Password reset success.", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }


}
