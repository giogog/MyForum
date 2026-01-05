using Contracts;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers;

public class CommentController(IServiceManager _serviceManager):ApiController(_serviceManager)
{
    [HttpGet("{topicid}")]
    public async Task<IActionResult> GetAllComments(int topicId)
    {
        var topics = await _serviceManager.CommentService.GetAllComment(topicId);

        _response = new ApiResponse("Comments", true, topics, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }
    [HttpGet("paged/{topicid}")]
    public async Task<IActionResult> GetComments(int topicId,[FromBody]int page)
    {
        var topics = await _serviceManager.CommentService.GetCommentByPage(page,topicId);

        _response = new PaginatedApiResponse("Comments by page", true, topics, Convert.ToInt32(HttpStatusCode.OK), topics.SelectedPage, topics.TotalPages, topics.PageSize, topics.ItemCount);
        return StatusCode(_response.StatusCode, _response);
    }


    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreateComment([FromBody]CreateCommentDto commentDto)
    {
        var user = await _serviceManager.UserService.GetUserWithClaim(User);
        await _serviceManager.CommentService.CreateComment(user.Id,commentDto);

        _response = new ApiResponse("Comment Added Succesfully", true, null, Convert.ToInt32(HttpStatusCode.Created));
        return StatusCode(_response.StatusCode, _response);

    }
    [Authorize]
    [HttpPut("update/{commentId}")]
    public async Task<IActionResult> UpdateComment([FromBody] UpdateCommentDto commentDto,int commentId)
    {
        var user = await _serviceManager.UserService.GetUserWithClaim(User);
        await _serviceManager.CommentService.UpdateComment(user.Id, commentId, commentDto);

        _response = new ApiResponse("Comment updated Succesfully", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);

    }
    [Authorize]
    [HttpDelete("delete/{commentId}")]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        var user = await _serviceManager.UserService.GetUserWithClaim(User);
        await _serviceManager.CommentService.DeleteComment(user.Id, commentId);

        _response = new ApiResponse("Comment deleted Succesfully", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);

    }



}
