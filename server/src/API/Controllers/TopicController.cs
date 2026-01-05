using Contracts;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace API.Controllers;


public class TopicController(IServiceManager _serviceManager) : ApiController(_serviceManager)
{
    [HttpGet("{forumId}/{page}")]
    public async Task<IActionResult> GetTopicsByForum(int forumId, int page)
    {
        var topics = await _serviceManager.TopicService.GetForumTopicsByPage(forumId,page);

        _response = new PaginatedApiResponse("Topics", true, topics, Convert.ToInt32(HttpStatusCode.OK), topics.SelectedPage, topics.TotalPages, topics.PageSize, topics.ItemCount);
        return StatusCode(_response.StatusCode, _response);
    }

    [HttpGet("topics/{page}")]
    public async Task<IActionResult> GetAllTopics(int page)
    {
        var topics = await _serviceManager.TopicService.GetAllTopicsByPage(page);

        _response = new PaginatedApiResponse("All Topics", true, topics, Convert.ToInt32(HttpStatusCode.OK), topics.SelectedPage, topics.TotalPages, topics.PageSize, topics.ItemCount);
        return StatusCode(_response.StatusCode, _response);
    }

    [Authorize]
    [HttpGet("user/{userId}/{page}")]
    public async Task<IActionResult> GetTopicsWithUserId(int userId, int page)
    {
        var topics = await _serviceManager.TopicService.GetTopicsWithUserIdByPage(userId, page);

        _response = new PaginatedApiResponse("Pending Topics with user Id", true, topics, Convert.ToInt32(HttpStatusCode.OK), topics.SelectedPage, topics.TotalPages, topics.PageSize, topics.ItemCount);
        return StatusCode(_response.StatusCode, _response);
    }

    [Authorize(Roles = "Admin,Moderator")]
    [HttpGet("pending/{page}")]
    public async Task<IActionResult> GetPendingTopics(int page)
    {
        var topics = await _serviceManager.TopicService.GetPendingTopicsByPage(page);

        _response = new PaginatedApiResponse("Pending Topics", true, topics, Convert.ToInt32(HttpStatusCode.OK), topics.SelectedPage, topics.TotalPages, topics.PageSize, topics.ItemCount);
        return StatusCode(_response.StatusCode, _response);
    }

    [HttpGet("topic/{topicId}")]
    public async Task<IActionResult> GetSingleTopic(int topicId)
    {
        var topic = await _serviceManager.TopicService.GetSingleTopic(topicId);

        _response = new ApiResponse("Topics", true, topic, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }


    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> AddTopic([FromBody] CreateTopicDto createTopicDto) 
    {
        var user = await _serviceManager.UserService.GetUserWithClaim(User);
        await _serviceManager.TopicService.CreateTopic(user.Id,createTopicDto);

        _response = new ApiResponse("Topic add Successfully", true, null, Convert.ToInt32(HttpStatusCode.Created));
        return StatusCode(_response.StatusCode, _response);
    }


    [Authorize]
    [HttpPut("update/{topicId}")]
    public async Task<IActionResult> UpdateTopic([FromBody] UpdateTopicDto updateTopicDto, int topicId)
    {
        await _serviceManager.TopicService.UpdateTopic(topicId, updateTopicDto);
        _response = new ApiResponse("Topic Updated Successfully", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }
    [Authorize]
    [HttpDelete("delete/{topicId}")]
    public async Task<IActionResult> DeleteTopic(int topicId)
    {

        await _serviceManager.TopicService.DeleteTopic(topicId);
        _response = new ApiResponse("Topic Deleted Successfully", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }


    [Authorize(Roles = "Admin,Moderator")]
    [HttpPatch("change-topic-status/{topicId}/{status}")]
    public async Task<IActionResult> ChangeTopicStatus(int topicId, Status status) 
    {
        await _serviceManager.TopicService.ChangeTopicStatus(topicId, status);
        _response = new ApiResponse($"Topic status updated to: {status.ToString()}", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }

    [Authorize(Roles = "Admin,Moderator")]
    [HttpPatch("change-topic-state/{topicId}/{state}")]
    public async Task<IActionResult> ChangeTopicState(int topicId, State state)
    {
        await _serviceManager.TopicService.ChangeTopicState(topicId, state);
        _response = new ApiResponse($"Topic status updated to: {state.ToString()}", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }

}
