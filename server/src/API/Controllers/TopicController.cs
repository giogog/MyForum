using Contracts;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace API.Controllers;


/// <summary>
/// Manages topic operations including creation, updates, moderation, and retrieval.
/// </summary>
public class TopicController(IServiceManager _serviceManager) : ApiController(_serviceManager)
{
    /// <summary>
    /// Retrieves all topics for a specific forum with pagination.
    /// </summary>
    /// <param name="forumId">The unique identifier of the forum.</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <returns>Paginated list of topics for the specified forum.</returns>
    /// <response code="200">Returns paginated topics.</response>
    /// <response code="404">Forum not found.</response>
    [HttpGet("{forumId}/{page}")]
    [ProducesResponseType(typeof(PaginatedApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaginatedApiResponse>> GetTopicsByForum(int forumId, int page)
    {
        var topics = await _serviceManager.TopicService.GetForumTopicsByPage(forumId,page);

        _response = new PaginatedApiResponse("Topics", true, topics, Convert.ToInt32(HttpStatusCode.OK), topics.SelectedPage, topics.TotalPages, topics.PageSize, topics.ItemCount);
        return StatusCode(_response.StatusCode, _response);
    }

    /// <summary>
    /// Retrieves all topics with pagination (Public access).
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <returns>Paginated list of all topics.</returns>
    /// <response code="200">Returns paginated topics.</response>
    [HttpGet("topics/{page}")]
    [ProducesResponseType(typeof(PaginatedApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedApiResponse>> GetAllTopics(int page)
    {
        var topics = await _serviceManager.TopicService.GetAllTopicsByPage(page);

        _response = new PaginatedApiResponse("All Topics", true, topics, Convert.ToInt32(HttpStatusCode.OK), topics.SelectedPage, topics.TotalPages, topics.PageSize, topics.ItemCount);
        return StatusCode(_response.StatusCode, _response);
    }

    /// <summary>
    /// Retrieves all topics created by a specific user with pagination (Authenticated users).
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <returns>Paginated list of topics created by the specified user.</returns>
    /// <response code="200">Returns paginated topics.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="404">User not found.</response>
    [Authorize]
    [HttpGet("user/{userId}/{page}")]
    [ProducesResponseType(typeof(PaginatedApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaginatedApiResponse>> GetTopicsWithUserId(int userId, int page)
    {
        var topics = await _serviceManager.TopicService.GetTopicsWithUserIdByPage(userId, page);

        _response = new PaginatedApiResponse("Pending Topics with user Id", true, topics, Convert.ToInt32(HttpStatusCode.OK), topics.SelectedPage, topics.TotalPages, topics.PageSize, topics.ItemCount);
        return StatusCode(_response.StatusCode, _response);
    }

    /// <summary>
    /// Retrieves all pending topics awaiting approval with pagination (Admin/Moderator only).
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <returns>Paginated list of pending topics.</returns>
    /// <response code="200">Returns paginated pending topics.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - admin or moderator role required.</response>
    [Authorize(Roles = "Admin,Moderator")]
    [HttpGet("pending/{page}")]
    [ProducesResponseType(typeof(PaginatedApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginatedApiResponse>> GetPendingTopics(int page)
    {
        var topics = await _serviceManager.TopicService.GetPendingTopicsByPage(page);

        _response = new PaginatedApiResponse("Pending Topics", true, topics, Convert.ToInt32(HttpStatusCode.OK), topics.SelectedPage, topics.TotalPages, topics.PageSize, topics.ItemCount);
        return StatusCode(_response.StatusCode, _response);
    }

    /// <summary>
    /// Retrieves a single topic by its identifier.
    /// </summary>
    /// <param name="topicId">The unique identifier of the topic.</param>
    /// <returns>The topic details.</returns>
    /// <response code="200">Returns the topic.</response>
    /// <response code="404">Topic not found.</response>
    [HttpGet("topic/{topicId}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> GetSingleTopic(int topicId)
    {
        var topic = await _serviceManager.TopicService.GetSingleTopic(topicId);

        _response = new ApiResponse("Topics", true, topic, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }


    /// <summary>
    /// Creates a new topic in a forum (Authenticated users).
    /// </summary>
    /// <param name="createTopicDto">The topic creation data.</param>
    /// <returns>Topic creation confirmation.</returns>
    /// <response code="201">Topic created successfully.</response>
    /// <response code="400">Invalid topic data.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="404">Forum not found.</response>
    [Authorize]
    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> AddTopic([FromBody] CreateTopicDto createTopicDto) 
    {
        var user = await _serviceManager.UserService.GetUserWithClaim(User);
        await _serviceManager.TopicService.CreateTopic(user.Id,createTopicDto);

        _response = new ApiResponse("Topic add Successfully", true, null, Convert.ToInt32(HttpStatusCode.Created));
        return StatusCode(_response.StatusCode, _response);
    }


    /// <summary>
    /// Updates an existing topic (Authenticated users).
    /// </summary>
    /// <param name="updateTopicDto">The topic data to update.</param>
    /// <param name="topicId">The unique identifier of the topic to update.</param>
    /// <returns>Update confirmation.</returns>
    /// <response code="200">Topic updated successfully.</response>
    /// <response code="400">Invalid topic data.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - user does not own this topic.</response>
    /// <response code="404">Topic not found.</response>
    [Authorize]
    [HttpPut("update/{topicId}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UpdateTopic([FromBody] UpdateTopicDto updateTopicDto, int topicId)
    {
        await _serviceManager.TopicService.UpdateTopic(topicId, updateTopicDto);
        _response = new ApiResponse("Topic Updated Successfully", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }
    /// <summary>
    /// Deletes a topic (Authenticated users).
    /// </summary>
    /// <param name="topicId">The unique identifier of the topic to delete.</param>
    /// <returns>Deletion confirmation.</returns>
    /// <response code="200">Topic deleted successfully.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - user does not own this topic.</response>
    /// <response code="404">Topic not found.</response>
    [Authorize]
    [HttpDelete("delete/{topicId}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeleteTopic(int topicId)
    {

        await _serviceManager.TopicService.DeleteTopic(topicId);
        _response = new ApiResponse("Topic Deleted Successfully", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }


    /// <summary>
    /// Changes the status of a topic (Admin/Moderator only).
    /// </summary>
    /// <param name="topicId">The unique identifier of the topic.</param>
    /// <param name="status">The new status to set.</param>
    /// <returns>Status change confirmation.</returns>
    /// <response code="200">Topic status updated successfully.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - admin or moderator role required.</response>
    /// <response code="404">Topic not found.</response>
    [Authorize(Roles = "Admin,Moderator")]
    [HttpPatch("change-topic-status/{topicId}/{status}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> ChangeTopicStatus(int topicId, Status status) 
    {
        await _serviceManager.TopicService.ChangeTopicStatus(topicId, status);
        _response = new ApiResponse($"Topic status updated to: {status.ToString()}", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }

    /// <summary>
    /// Changes the state of a topic (Admin/Moderator only).
    /// </summary>
    /// <param name="topicId">The unique identifier of the topic.</param>
    /// <param name="state">The new state to set.</param>
    /// <returns>State change confirmation.</returns>
    /// <response code="200">Topic state updated successfully.</response>
    /// <response code="401">Unauthorized - authentication required.</response>
    /// <response code="403">Forbidden - admin or moderator role required.</response>
    /// <response code="404">Topic not found.</response>
    [Authorize(Roles = "Admin,Moderator")]
    [HttpPatch("change-topic-state/{topicId}/{state}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> ChangeTopicState(int topicId, State state)
    {
        await _serviceManager.TopicService.ChangeTopicState(topicId, state);
        _response = new ApiResponse($"Topic status updated to: {state.ToString()}", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }

}
