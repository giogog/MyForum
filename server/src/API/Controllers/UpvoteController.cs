using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers;

public class UpvoteController(IServiceManager _serviceManager) : ApiController(_serviceManager)
{
    [Authorize]
    [HttpPost("{topicId}")]
    public async Task<IActionResult> UpVote(int topicId)
    {
        var user = await _serviceManager.UserService.GetUserWithClaim(User);
        await _serviceManager.UpvoteService.Upvote(user.Id, topicId);

        _response = new ApiResponse("Topic Upvoted Succesfully", true, null, Convert.ToInt32(HttpStatusCode.Created));
        return StatusCode(_response.StatusCode, _response);

    }
}

