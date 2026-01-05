using Contracts;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers;

public class ReplyController(IServiceManager _serviceManager) : ApiController(_serviceManager)
{
    [Authorize(Roles = "User")]
    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto commentDto)
    {
        var user = await _serviceManager.UserService.GetUserWithClaim(User);
        await _serviceManager.CommentService.CreateComment(user.Id, commentDto);

        _response = new ApiResponse("Comment Added Succesfully", true, null, Convert.ToInt32(HttpStatusCode.Created));
        return StatusCode(_response.StatusCode, _response);

    }
}
