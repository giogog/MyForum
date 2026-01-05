using Domain.Entities;
using Domain.Models;
using System.Security.Claims;

namespace Contracts;

public interface IUserService
{
    Task<User> GetUserWithClaim(ClaimsPrincipal principal);
    Task<AuthorizedUserDto> GetUserWithEmail(string email);
    Task<AuthorizedUserDto> GetAuthorizedUserData(int id);
    Task UpdateAuthorizedUser(int id, AuthorizedUserDto authorizedUserDto);
    Task<PagedList<AuthorizedUserDto>> GetUsers(int page);
    Task UserBanStatusChange(int userId, Ban ban);
    Task UserModeratorStatus(int userId, string role);
    Task<IEnumerable<string>> GetUserRoles(int userId);
} 
