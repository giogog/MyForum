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
    public async Task<IActionResult> GetAllForums(int page)
    {
        var forums = await _serviceManager.ForumService.GetAllForumsByPage(page);

        _response = new PaginatedApiResponse("All Forums", true, forums, Convert.ToInt32(HttpStatusCode.OK), forums.SelectedPage, forums.TotalPages, forums.PageSize, forums.ItemCount);
        return StatusCode(_response.StatusCode, _response);
    }
    [Authorize(Roles = "Admin")]
    [HttpGet("pending/{page}")]
    public async Task<IActionResult> GetPendingForums(int page)
    {
        var forums = await _serviceManager.ForumService.GetPendingForums(page);

        _response = new PaginatedApiResponse("Pending Forums", true, forums, Convert.ToInt32(HttpStatusCode.OK), forums.SelectedPage, forums.TotalPages, forums.PageSize, forums.ItemCount);
        return StatusCode(_response.StatusCode, _response);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("deleted/{page}")]
    public async Task<IActionResult> GetDeletedForums(int page)
    {
        var forums = await _serviceManager.ForumService.GetDeletedForums(page);

        _response = new PaginatedApiResponse("Delete Forums", true, forums, Convert.ToInt32(HttpStatusCode.OK), forums.SelectedPage, forums.TotalPages, forums.PageSize, forums.ItemCount);
        return StatusCode(_response.StatusCode, _response);
    }
    [HttpGet("forums/{page}")]
    public async Task<IActionResult> GetForums(int page)
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
    public async Task<IActionResult> CreateForum([FromBody] CreateForumDto createForumDto)
    {
        var user = await _serviceManager.UserService.GetUserWithClaim(User);
        await _serviceManager.ForumService.CreateForum(user.Id,createForumDto);

        _response = new ApiResponse("Forum created Successfully", true, null, Convert.ToInt32(HttpStatusCode.Created));
        return StatusCode(_response.StatusCode, _response);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("delete/{forumId}")]
    public async Task<IActionResult> DeletForum(int forumId)
    {
        await _serviceManager.ForumService.DeleteForum(forumId);

        _response = new ApiResponse("Forum Deleted Successfully", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }

    [Authorize]
    [HttpPatch("delete-from-user/{forumId}")]
    public async Task<IActionResult> DeletForumFromUser(int forumId)
    {
        await _serviceManager.ForumService.DeleteForumFromUser(forumId);

        _response = new ApiResponse("Forum Deleted Successfully", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }

    [Authorize]
    [HttpPut("update/{forumId}")]
    public async Task<IActionResult> UpdateForum([FromBody]UpdateForumDto updateForumDto , int forumId)
    {
        await _serviceManager.ForumService.UpdateForum(forumId, updateForumDto);

        _response = new ApiResponse("Forum Updated Successfully", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("state/{forumId}/{state}")]
    public async Task<IActionResult> ChangeState(int forumId, State state)
    {
        await _serviceManager.ForumService.ChangeForumState(forumId, state);

        _response = new ApiResponse($"Forum State Updated Successfully to {nameof(state)}", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("status/{forumId}/{status}")]
    public async Task<IActionResult> ChangeStatus(int forumId, Status status)
    {
        await _serviceManager.ForumService.ChangeForumStatus(forumId, status);

        _response = new ApiResponse($"Forum Status Updated Successfully to {nameof(status)}", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }

}
