using Contracts;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers;

/// <summary>
/// Manages comment operations including creation, updates, deletion, and retrieval.
/// </summary>
public class CommentController(IServiceManager _serviceManager):ApiController(_serviceManager)
{
    /// <summary>
    /// Retrieves all comments for a specific topic.
    /// </summary>
    /// <param name="topicId">The unique identifier of the topic.</param>
    /// <returns>List of all comments for the specified topic.</returns>
    /// <response code="200">Returns the list of comments.</response>
    /// <response code="404">Topic not found.</response>
    [HttpGet("{topicid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> GetAllComments(int topicId)
    {
        var topics = await _serviceManager.CommentService.GetAllComment(topicId);

        _response = new ApiResponse("Comments", true, topics, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }
    /// <summary>
    /// Retrieves comments for a specific topic with pagination.
    /// </summary>
    /// <param name="topicId">The unique identifier of the topic.</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <returns>Paginated list of comments for the specified topic.</returns>
    /// <response code="200">Returns paginated comments.</response>
    /// <response code="404">Topic not found.</response>
    [HttpGet("paged/{topicid}")]
    [ProducesResponseType(typeof(PaginatedApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaginatedApiResponse>> GetComments(int topicId,[FromBody]int page)
    {
        var topics = await _serviceManager.CommentService.GetCommentByPage(page,topicId);

        _response = new PaginatedApiResponse("Comments by page", true, topics, Convert.ToInt32(HttpStatusCode.OK), topics.SelectedPage, topics.TotalPages, topics.PageSize, topics.ItemCount);
        return StatusCode(_response.StatusCode, _response);
    }


    /// <summary>
    /// Creates a new comment on a topic (Authenticated users).
    /// </summary>
    /// <param name="createCommentDto">The comment creation data.</param>
    /// <returns>Comment creation confirmation.</returns>
    /// <response code="201">Comment created successfully.</response>
    /// <response code="400">Invalid comment data.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="404">Topic not found.</response>
    [Authorize]
    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> CreateComment([FromBody] CreateCommentDto createCommentDto)
    {
        var user = await _serviceManager.UserService.GetUserWithClaim(User);
        await _serviceManager.CommentService.CreateComment(user.Id,createCommentDto);

        _response = new ApiResponse("Comment Added Succesfully", true, null, Convert.ToInt32(HttpStatusCode.Created));
        return StatusCode(_response.StatusCode, _response);

    }
    /// <summary>
    /// Updates an existing comment (Authenticated users).
    /// </summary>
    /// <param name="updateCommentDto">The comment data to update.</param>
    /// <param name="commentId">The unique identifier of the comment to update.</param>
    /// <returns>Update confirmation.</returns>
    /// <response code="200">Comment updated successfully.</response>
    /// <response code="400">Invalid comment data.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - user does not own this comment.</response>
    /// <response code="404">Comment not found.</response>
    [Authorize]
    [HttpPut("update/{commentId}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UpdateComment([FromBody] UpdateCommentDto updateCommentDto, int commentId)
    {
        var user = await _serviceManager.UserService.GetUserWithClaim(User);
        await _serviceManager.CommentService.UpdateComment(user.Id, commentId, updateCommentDto);

        _response = new ApiResponse("Comment updated Succesfully", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);

    }
    /// <summary>
    /// Deletes a comment (Authenticated users).
    /// </summary>
    /// <param name="commentId">The unique identifier of the comment to delete.</param>
    /// <returns>Deletion confirmation.</returns>
    /// <response code="200">Comment deleted successfully.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - user does not own this comment.</response>
    /// <response code="404">Comment not found.</response>
    [Authorize]
    [HttpDelete("delete/{commentId}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeleteComment(int commentId)
    {
        var user = await _serviceManager.UserService.GetUserWithClaim(User);
        await _serviceManager.CommentService.DeleteComment(user.Id, commentId);

        _response = new ApiResponse("Comment deleted Succesfully", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);

    }



}
