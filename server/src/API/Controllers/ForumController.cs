using Contracts;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers;

public class ForumController(IServiceManager _serviceManager) : ApiController(_serviceManager)
{
    [Authorize(Roles = "Admin")]
    [HttpGet("{page}")]
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


    [Authorize]
    [HttpPost("create")]
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
