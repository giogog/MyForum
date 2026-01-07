using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers;

/// <summary>
/// Manages upvote operations for topics.
/// </summary>
public class UpvoteController(IServiceManager _serviceManager) : ApiController(_serviceManager)
{
    /// <summary>
    /// Upvotes a topic (Authenticated users).
    /// </summary>
    /// <param name="topicId">The unique identifier of the topic to upvote.</param>
    /// <returns>Upvote confirmation.</returns>
    /// <response code="201">Topic upvoted successfully.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="404">Topic not found.</response>
    /// <response code="409">Conflict - user already upvoted this topic.</response>
    [Authorize]
    [HttpPost("{topicId}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse>> UpVote(int topicId)
    {
        var user = await _serviceManager.UserService.GetUserWithClaim(User);
        await _serviceManager.UpvoteService.Upvote(user.Id, topicId);

        _response = new ApiResponse("Topic Upvoted Succesfully", true, null, Convert.ToInt32(HttpStatusCode.Created));
        return StatusCode(_response.StatusCode, _response);

    }
}

