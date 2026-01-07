using Contracts;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers;

/// <summary>
/// Manages forum operations including CRUD and moderation
/// </summary>
public class ForumController(IServiceManager _serviceManager) : ApiController(_serviceManager)
{
    /// <summary>
    /// Get all forums with pagination (Admin only)
    /// </summary>
    /// <param name="page">Page number</param>
    /// <returns>Paginated list of all forums</returns>
    /// <response code="200">Returns paginated forums</response>
    /// <response code="401">Unauthorized - authentication required</response>
    /// <response code="403">Forbidden - admin role required</response>
    [Authorize(Roles = "Admin")]
    [HttpGet("{page}")]
    [ProducesResponseType(typeof(PaginatedApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginatedApiResponse>> GetAllForums(int page)
    {
        var forums = await _serviceManager.ForumService.GetAllForumsByPage(page);

        _response = new PaginatedApiResponse("All Forums", true, forums, Convert.ToInt32(HttpStatusCode.OK), forums.SelectedPage, forums.TotalPages, forums.PageSize, forums.ItemCount);
        return StatusCode(_response.StatusCode, _response);
    }
    /// <summary>
    /// Retrieves all forums pending approval with pagination (Admin only).
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <returns>Paginated list of pending forums.</returns>
    /// <response code="200">Returns paginated pending forums.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - admin role required.</response>
    [Authorize(Roles = "Admin")]
    [HttpGet("pending/{page}")]
    [ProducesResponseType(typeof(PaginatedApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginatedApiResponse>> GetPendingForums(int page)
    {
        var forums = await _serviceManager.ForumService.GetPendingForums(page);

        _response = new PaginatedApiResponse("Pending Forums", true, forums, Convert.ToInt32(HttpStatusCode.OK), forums.SelectedPage, forums.TotalPages, forums.PageSize, forums.ItemCount);
        return StatusCode(_response.StatusCode, _response);
    }

    /// <summary>
    /// Retrieves all deleted forums with pagination (Admin only).
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <returns>Paginated list of deleted forums.</returns>
    /// <response code="200">Returns paginated deleted forums.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - admin role required.</response>
    [Authorize(Roles = "Admin")]
    [HttpGet("deleted/{page}")]
    [ProducesResponseType(typeof(PaginatedApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginatedApiResponse>> GetDeletedForums(int page)
    {
        var forums = await _serviceManager.ForumService.GetDeletedForums(page);

        _response = new PaginatedApiResponse("Delete Forums", true, forums, Convert.ToInt32(HttpStatusCode.OK), forums.SelectedPage, forums.TotalPages, forums.PageSize, forums.ItemCount);
        return StatusCode(_response.StatusCode, _response);
    }
    /// <summary>
    /// Retrieves all active approved forums with pagination (Public access).
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <returns>Paginated list of active forums.</returns>
    /// <response code="200">Returns paginated active forums.</response>
    [HttpGet("forums/{page}")]
    [ProducesResponseType(typeof(PaginatedApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedApiResponse>> GetForums(int page)
    {
        var forums = await _serviceManager.ForumService.GetForumsByPage(page);

        _response = new PaginatedApiResponse("Forums", true, forums, Convert.ToInt32(HttpStatusCode.OK), forums.SelectedPage, forums.TotalPages, forums.PageSize, forums.ItemCount);
        return StatusCode(_response.StatusCode, _response);
    }

    /// <summary>
    /// Create a new forum (Authenticated users)
    /// </summary>
    /// <param name="createForumDto">Forum creation data</param>
    /// <returns>Forum creation status</returns>
    /// <response code="201">Forum created successfully</response>
    /// <response code="400">Invalid forum data</response>
    /// <response code="401">Unauthorized - authentication required</response>
    [Authorize]
    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse>> CreateForum([FromBody] CreateForumDto createForumDto)
    {
        var user = await _serviceManager.UserService.GetUserWithClaim(User);
        await _serviceManager.ForumService.CreateForum(user.Id,createForumDto);

        _response = new ApiResponse("Forum created Successfully", true, null, Convert.ToInt32(HttpStatusCode.Created));
        return StatusCode(_response.StatusCode, _response);
    }

    /// <summary>
    /// Permanently deletes a forum (Admin only).
    /// </summary>
    /// <param name="forumId">The unique identifier of the forum to delete.</param>
    /// <returns>Deletion confirmation.</returns>
    /// <response code="200">Forum deleted successfully.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - admin role required.</response>
    /// <response code="404">Forum not found.</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("delete/{forumId}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeletForum(int forumId)
    {
        await _serviceManager.ForumService.DeleteForum(forumId);

        _response = new ApiResponse("Forum Deleted Successfully", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }

    /// <summary>
    /// Marks a forum as deleted by the owning user (Authenticated users).
    /// </summary>
    /// <param name="forumId">The unique identifier of the forum to mark as deleted.</param>
    /// <returns>Deletion confirmation.</returns>
    /// <response code="200">Forum marked as deleted successfully.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - user does not own this forum.</response>
    /// <response code="404">Forum not found.</response>
    [Authorize]
    [HttpPatch("delete-from-user/{forumId}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeletForumFromUser(int forumId)
    {
        await _serviceManager.ForumService.DeleteForumFromUser(forumId);

        _response = new ApiResponse("Forum Deleted Successfully", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }

    /// <summary>
    /// Updates an existing forum (Authenticated users).
    /// </summary>
    /// <param name="updateForumDto">The forum data to update.</param>
    /// <param name="forumId">The unique identifier of the forum to update.</param>
    /// <returns>Update confirmation.</returns>
    /// <response code="200">Forum updated successfully.</response>
    /// <response code="400">Invalid forum data.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - user does not own this forum.</response>
    /// <response code="404">Forum not found.</response>
    [Authorize]
    [HttpPut("update/{forumId}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UpdateForum([FromBody]UpdateForumDto updateForumDto , int forumId)
    {
        await _serviceManager.ForumService.UpdateForum(forumId, updateForumDto);

        _response = new ApiResponse("Forum Updated Successfully", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }

    /// <summary>
    /// Changes the state of a forum (Admin only).
    /// </summary>
    /// <param name="forumId">The unique identifier of the forum.</param>
    /// <param name="state">The new state to set.</param>
    /// <returns>State change confirmation.</returns>
    /// <response code="200">Forum state updated successfully.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - admin role required.</response>
    /// <response code="404">Forum not found.</response>
    [Authorize(Roles = "Admin")]
    [HttpPatch("state/{forumId}/{state}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> ChangeState(int forumId, State state)
    {
        await _serviceManager.ForumService.ChangeForumState(forumId, state);

        _response = new ApiResponse($"Forum State Updated Successfully to {nameof(state)}", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }

    /// <summary>
    /// Changes the status of a forum (Admin only).
    /// </summary>
    /// <param name="forumId">The unique identifier of the forum.</param>
    /// <param name="status">The new status to set.</param>
    /// <returns>Status change confirmation.</returns>
    /// <response code="200">Forum status updated successfully.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - admin role required.</response>
    /// <response code="404">Forum not found.</response>
    [Authorize(Roles = "Admin")]
    [HttpPatch("status/{forumId}/{status}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> ChangeStatus(int forumId, Status status)
    {
        await _serviceManager.ForumService.ChangeForumStatus(forumId, status);

        _response = new ApiResponse($"Forum Status Updated Successfully to {nameof(status)}", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }

}
