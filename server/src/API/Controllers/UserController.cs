using Contracts;
using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers;

public class UserController(IServiceManager _serviceManager) : ApiController(_serviceManager)
{
    [Authorize(Roles ="Admin")]
    [HttpGet("users/{page}")]
    public async Task<IActionResult> GetUsers(int page)
    {
        var users = await _serviceManager.UserService.GetUsers(page);

        _response = new PaginatedApiResponse("Pending Topics with user Id", true, users, Convert.ToInt32(HttpStatusCode.OK), users.SelectedPage, users.TotalPages, users.PageSize, users.ItemCount);
        return StatusCode(_response.StatusCode, _response);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("roles/{userId}")]
    public async Task<IActionResult> GetUserRoles(int userId)
    {
        var users = await _serviceManager.UserService.GetUserRoles(userId);

        _response = new ApiResponse("UserRoles", true, users, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }


    [Authorize]
    [HttpGet("with-email/{email}")]
    public async Task<IActionResult> GetUserWithEmail(string email)
    {
        var user = await _serviceManager.UserService.GetUserWithEmail(email);

        _response = new ApiResponse("User with Email", true, user, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }

    [Authorize]
    [HttpGet("with-id/{id}")]
    public async Task<IActionResult> GetAuthorizedUserData(int id)
    {
        var user = await _serviceManager.UserService.GetAuthorizedUserData(id);

        _response = new ApiResponse("Authorized User Data", true, user, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }
    [Authorize]
    [HttpPut("with-id/{id}")]
    public async Task<IActionResult> UpdateAuthorizedUserData(int id, [FromBody] AuthorizedUserDto authorizedUserDto)
    {
        await _serviceManager.UserService.UpdateAuthorizedUser(id, authorizedUserDto);

        _response = new ApiResponse("User Updated Succesfully", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{userId}/{ban}")]
    public async Task<IActionResult> BanUser(int userId ,Ban ban)
    {
        await _serviceManager.UserService.UserBanStatusChange(userId, ban);

        _response = new ApiResponse($"User {ban} Successfully", true, null, Convert.ToInt32(HttpStatusCode.OK));
        return StatusCode(_response.StatusCode, _response);
    }
    [Authorize(Roles = "Admin")]
    [HttpPost("moderator/{userId}")]
    public async Task<IActionResult> UserModeratorRole(int userId)
    {
        await _serviceManager.UserService.UserModeratorStatus(userId, "Moderator");
        _response = new ApiResponse($"User Moderator status changed Successfully", true, null, Convert.ToInt32(HttpStatusCode.Created));
        return StatusCode(_response.StatusCode, _response);
    }



}
