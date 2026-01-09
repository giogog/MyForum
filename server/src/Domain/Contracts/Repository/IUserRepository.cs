using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Contracts;

public interface IUserRepository
{
    Task<IdentityResult> CreateUser(User user, string passord);
    Task<User> GetUser(Expression<Func<User, bool>> expression);
    Task<IEnumerable<User>> GetAllUsers();
    Task<IEnumerable<User>> GetUsersWithCondition(Expression<Func<User, bool>> expression);

    Task<bool> CheckPassword(User user,string password);
    Task<User> GetUserFromClaim(ClaimsPrincipal claimsPrincipal);
    Task UpdateUser(User user);
    void CheckUserBanStatus(User user);
    Task<IdentityResult> AddToRole(User user, string role);
    Task<bool> UserRoleExists(string roleName);
    Task<IdentityResult> CreateUserRole(Role role);
    Task<IdentityResult> DeleteUserRole(User user, string role);
    Task<Role> FindUserRolebyName(string role);
    Task<Role> FindUserRoleById(string role);
    Task<string> GetRoleName(Role role);

    IQueryable<User> Users();
}
