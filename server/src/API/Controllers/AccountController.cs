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
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var registrationCheckUp = await _serviceManager.AuthorizationService.Register(registerDto);

        if (!registrationCheckUp.Succeeded)
        {

            _response = new ApiResponse(registrationCheckUp.Errors.First().Description, false, null, Convert.ToInt32(HttpStatusCode.BadRequest));
            return StatusCode(_response.StatusCode, _response);
        }


        var emailSent = await _serviceManager.EmailService.SendConfirmationMail(Url, registerDto.Username);

        if (!emailSent.IsSuccess){
            _response = new ApiResponse(emailSent.ErrorMessage, false, null, Convert.ToInt32(HttpStatusCode.BadRequest));
            return StatusCode(_response.StatusCode, _response);
        }
        _response = new ApiResponse("Registration successful. Please check your email to confirm your account.", true, null, Convert.ToInt32(HttpStatusCode.Created));
        return StatusCode(_response.StatusCode, _response);
    }
    /// <summary>
    /// Authenticate user and receive JWT token
    /// </summary>
    /// <param name="loginDto">Login credentials</param>
    /// <returns>JWT token for authenticated requests</returns>
    /// <response code="200">Login successful, returns JWT token</response>
    /// <response code="400">Invalid credentials or email not confirmed</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(LoginDto loginDto)
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
    /// Confirm user email address via token
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="token">Email confirmation token</param>
    /// <returns>Redirect to login or error page</returns>
    /// <response code="302">Redirects to appropriate page based on confirmation result</response>
    [HttpGet("confirm-email")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    public async Task<IActionResult> ConfirmEmail(int userId, string token)
    {
        var result = await _serviceManager.EmailService.ConfirmEmailAsync(userId.ToString(), token);

        if (!result.Succeeded)
        {
            return Redirect($"https://localhost:5003/confirmation?errormassage={"Email confirmation Failed"}");
        }
        return Redirect("https://localhost:5003/login");

    }

    [HttpPost("resend-confirmation/{username}")]
    public async Task<IActionResult> ResendConfirmation(string username)
    {
        var alreadyConfirmed = await _serviceManager.EmailService.CheckMailConfirmation(username);

        if (alreadyConfirmed.IsSuccess)
        {
            _response = new ApiResponse(alreadyConfirmed.ErrorMessage, false, null, Convert.ToInt32(HttpStatusCode.BadRequest));
            return StatusCode(_response.StatusCode, _response);
        }


        var emailSent = await _serviceManager.EmailService.SendConfirmationMail(Url, username);

        if (!emailSent.IsSuccess)
        {
            _response = new ApiResponse(alreadyConfirmed.ErrorMessage, false, null, Convert.ToInt32(HttpStatusCode.BadRequest));
            return StatusCode(_response.StatusCode, _response);
        }

        _response = new ApiResponse("Please check your email to confirm your account.", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }

    [HttpPost]
    [Route("request-password-reset/{email}")]
    public async Task<IActionResult> RequestPasswordReset(string email)
    {
        var emailSent = await _serviceManager.EmailService.SendPasswordResetEmail(Url,email);

        if (!emailSent.IsSuccess)
        {
            _response = new ApiResponse(emailSent.ErrorMessage, false, null, Convert.ToInt32(HttpStatusCode.BadRequest));
            return StatusCode(_response.StatusCode, _response);
        }
        _response = new ApiResponse("Please check your email to for password reset.", true, null, Convert.ToInt32(HttpStatusCode.Created));
        return StatusCode(_response.StatusCode, _response);

    }

    [HttpGet]
    [Route("reset-password-page")]
    public async Task<IActionResult> ResetPasswordPage(string token, string email)
    {

        var encodedToken = Uri.EscapeDataString(token);
        return Redirect($"https://localhost:5003/reset-password?token={encodedToken}&email={Uri.EscapeDataString(email)}");
    }

    [HttpPut]
    [Route("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
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
