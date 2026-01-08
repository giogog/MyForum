using Contracts;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers;

/// <summary>
/// Manages reply operations for comments (nested comments).
/// </summary>
public class ReplyController(IServiceManager _serviceManager) : ApiController(_serviceManager)
{
    /// <summary>
    /// Creates a reply to an existing comment (Authenticated users).
    /// </summary>
    /// <param name="commentDto">The reply creation data.</param>
    /// <returns>Reply creation confirmation.</returns>
    /// <response code="201">Reply created successfully.</response>
    /// <response code="400">Invalid reply data.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="404">Parent comment not found.</response>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> CreateReply([FromBody] CreateCommentDto commentDto)
    {
        var user = await _serviceManager.UserService.GetUserWithClaim(User);
        await _serviceManager.CommentService.CreateComment(user.Id, commentDto);

        _response = new ApiResponse("Reply Added Succesfully", true, null, Convert.ToInt32(HttpStatusCode.Created));
        return StatusCode(_response.StatusCode, _response);

    }
}
