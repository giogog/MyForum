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
    /// <returns>Registration status</returns>
    /// <response code="201">User registered successfully</response>
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

        _response = new ApiResponse("Registration successful.", 
            true,
            await _serviceManager.AuthorizationService.Authenticate(user => user.UserName == registerDto.Username),
            Convert.ToInt32(HttpStatusCode.Created));
        return StatusCode(_response.StatusCode, _response);
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="loginDto">Login credentials containing username and password.</param>
    /// <returns>A JWT token and user information if authentication is successful.</returns>
    /// <response code="200">Returns the JWT token and user details.</response>
    /// <response code="400">If the credentials are invalid.</response>
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

}
