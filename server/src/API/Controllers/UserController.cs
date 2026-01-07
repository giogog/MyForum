using Contracts;
using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers;

/// <summary>
/// Manages user operations including profile management, role assignment, and moderation.
/// </summary>
public class UserController(IServiceManager _serviceManager) : ApiController(_serviceManager)
{
    /// <summary>
    /// Retrieves all users with pagination (Admin only).
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <returns>Paginated list of users.</returns>
    /// <response code="200">Returns paginated users.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - admin role required.</response>
    [Authorize(Roles ="Admin")]
    [HttpGet("users/{page}")]
    [ProducesResponseType(typeof(PaginatedApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginatedApiResponse>> GetUsers(int page)
    {
        var users = await _serviceManager.UserService.GetUsers(page);

        _response = new PaginatedApiResponse("Pending Topics with user Id", true, users, Convert.ToInt32(HttpStatusCode.OK), users.SelectedPage, users.TotalPages, users.PageSize, users.ItemCount);
        return StatusCode(_response.StatusCode, _response);
    }

    /// <summary>
    /// Retrieves all roles assigned to a specific user (Admin only).
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>List of user roles.</returns>
    /// <response code="200">Returns user roles.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - admin role required.</response>
    /// <response code="404">User not found.</response>
    [Authorize(Roles = "Admin")]
    [HttpGet("roles/{userId}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> GetUserRoles(int userId)
    {
        var users = await _serviceManager.UserService.GetUserRoles(userId);

        _response = new ApiResponse("UserRoles", true, users, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }


    /// <summary>
    /// Retrieves user information by email address (Authenticated users).
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <returns>User information.</returns>
    /// <response code="200">Returns user information.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="404">User not found.</response>
    [Authorize]
    [HttpGet("with-email/{email}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> GetUserWithEmail(string email)
    {
        var user = await _serviceManager.UserService.GetUserWithEmail(email);

        _response = new ApiResponse("User with Email", true, user, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }

    /// <summary>
    /// Retrieves authorized user data by user ID (Authenticated users).
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>Authorized user data.</returns>
    /// <response code="200">Returns authorized user data.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="404">User not found.</response>
    [Authorize]
    [HttpGet("with-id/{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> GetAuthorizedUserData(int id)
    {
        var user = await _serviceManager.UserService.GetAuthorizedUserData(id);

        _response = new ApiResponse("Authorized User Data", true, user, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }
    /// <summary>
    /// Updates authorized user profile data (Authenticated users).
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="authorizedUserDto">The user data to update.</param>
    /// <returns>Update confirmation.</returns>
    /// <response code="200">User updated successfully.</response>
    /// <response code="400">Invalid user data.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - user can only update their own profile.</response>
    /// <response code="404">User not found.</response>
    [Authorize]
    [HttpPut("with-id/{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UpdateAuthorizedUserData(int id, [FromBody] AuthorizedUserDto authorizedUserDto)
    {
        await _serviceManager.UserService.UpdateAuthorizedUser(id, authorizedUserDto);

        _response = new ApiResponse("User Updated Succesfully", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }

    /// <summary>
    /// Changes the ban status of a user (Admin only).
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="ban">The ban status to set.</param>
    /// <returns>Ban status change confirmation.</returns>
    /// <response code="200">User ban status updated successfully.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - admin role required.</response>
    /// <response code="404">User not found.</response>
    [Authorize(Roles = "Admin")]
    [HttpPatch("{userId}/{ban}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> BanUser(int userId ,Ban ban)
    {
        await _serviceManager.UserService.UserBanStatusChange(userId, ban);

        _response = new ApiResponse($"User {ban} Successfully", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }
    /// <summary>
    /// Assigns or revokes moderator role for a user (Admin only).
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>Moderator status change confirmation.</returns>
    /// <response code="201">User moderator status changed successfully.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - admin role required.</response>
    /// <response code="404">User not found.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("moderator/{userId}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UserModeratorRole(int userId)
    {
        await _serviceManager.UserService.UserModeratorStatus(userId, "Moderator");
        _response = new ApiResponse($"User Moderator status changed Successfully", true, null, Convert.ToInt32(HttpStatusCode.Created));
        return StatusCode(_response.StatusCode, _response);
    }



}
