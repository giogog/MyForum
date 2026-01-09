using AutoMapper;
using Contracts;
using Domain.Entities;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly int _pageSize;

        public UserService(IRepositoryManager repositoryManager, IMapper mapper, ILogger<UserService> logger,IConfiguration configuration)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _logger = logger;
            _pageSize = Int32.Parse(configuration["ApiSettings:PageSize"]!);
        }

        public async Task<PagedList<AuthorizedUserDto>> GetUsers(int page)
        {
            var users = _repositoryManager.UserRepository.Users()
                .AsNoTracking() // Read-only query optimization
                .Where(u => !u.Roles.Any(r => r.RoleId == -2))
                .Select(t => new AuthorizedUserDto(
                    t.Id,
                    t.Name, 
                    t.Surname, 
                    t.UserName!,
                    t.Email!,
                    t.Banned,
                    t.Roles.Select(r=>r.Role.Name!).ToArray()));

            return await PagedList<AuthorizedUserDto>.CreateAsync(users,page, _pageSize);
        }

        public async Task<IEnumerable<string>> GetUserRoles(int userId)
        {
            var user = await _repositoryManager.UserRepository.Users()
                .AsNoTracking() // Read-only query optimization
                .Where(u=>u.Id==userId)
                .Select(u => new 
                {
                    Roles = u.Roles.Select(ur => ur.Role.Name!).ToArray()
                })
                .SingleOrDefaultAsync();
                
            return user?.Roles ?? Array.Empty<string>();
        }

        public async Task<AuthorizedUserDto> GetAuthorizedUserData(int id)
        {
            var user = await _repositoryManager.UserRepository
                .GetUser(u => u.Id == id);
            if (user == null)
            {
                _logger.LogWarning("User not found with ID {Id}", id);
                throw new NotFoundException("User not found");
            }

            return _mapper.Map<AuthorizedUserDto>(user);
        }

        public async Task UpdateAuthorizedUser(int id, AuthorizedUserDto authorizedUserDto)
        {
            await _repositoryManager.BeginTransactionAsync();
            try
            {
                var user = await _repositoryManager.UserRepository
                    .GetUser(u => u.Id == id);
                if (user == null)
                {
                    _logger.LogWarning("User not found with ID {Id}", id);
                    throw new NotFoundException("User not found");
                }

                var checkUser = await _repositoryManager.UserRepository
                    .GetUser(u => u.UserName!.ToLower() == authorizedUserDto.Username.ToLower() && u.UserName != user.UserName);
                if (checkUser != null)
                {
                    _logger.LogWarning("Username {Username} is already taken.", authorizedUserDto.Username);
                    throw new UsernameIsTakenException("Username is already taken.");
                }

                user.Name = authorizedUserDto.Name;
                user.Surname = authorizedUserDto.Surname;
                user.UserName = authorizedUserDto.Username;
                user.SecurityStamp = Guid.NewGuid().ToString();

                await _repositoryManager.UserRepository.UpdateUser(user);
                await _repositoryManager.SaveAsync();
                await _repositoryManager.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating authorized user with ID {Id}", id);
                await _repositoryManager.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<AuthorizedUserDto> GetUserWithEmail(string email)
        {
            var user = await _repositoryManager.UserRepository
                .GetUser(u => u.Email == email );
            if (user == null)
            {
                _logger.LogWarning("User not found with email {Email}", email);
                throw new NotFoundException("User not found");
            }

            return _mapper.Map<AuthorizedUserDto>(user);
        }

        public async Task<User> GetUserWithClaim(ClaimsPrincipal principal)
        {
            var user = await _repositoryManager.UserRepository.GetUserFromClaim(principal);
            if (user == null)
            {
                _logger.LogWarning("User not found with the given claims.");
                throw new NotFoundException("User not found");
            }

            return user;
        }
        public async Task UserModeratorStatus(int userId,string role)
        {
            await _repositoryManager.BeginTransactionAsync();
            try
            {
                var userRole = await _repositoryManager.UserRepository.FindUserRolebyName(role);
                var user = await _repositoryManager.UserRepository.Users()
                    .Where(u=>u.Id==userId)
                    .Include(u=>u.Roles)
                        .ThenInclude(u=>u.Role)
                    .SingleOrDefaultAsync(); // Use SingleOrDefault for unique lookup
                
                if (user == null)
                {
                    _logger.LogWarning("User not found with ID {UserId}", userId);
                    throw new NotFoundException("User not found");
                }
                if(userRole == null)
                {
                    _logger.LogWarning("Role not found with name {Role}", role);
                    throw new NotFoundException("Role not found");
                }
                if (user.Roles.Any(u => u.Role == userRole))
                {
                    user.SecurityStamp = Guid.NewGuid().ToString();
                    var deleteRoleresult = await _repositoryManager.UserRepository.DeleteUserRole(user, userRole.Name!);
                    if (!deleteRoleresult.Succeeded)
                    {
                        throw new InvalidArgumentException(deleteRoleresult.Errors.First().ToString()!);
                    }


                }
                else
                {
                    user.SecurityStamp = Guid.NewGuid().ToString();
                    var addRoleresult = await _repositoryManager.UserRepository.AddToRole(user, role);
                    if (!addRoleresult.Succeeded)
                    {
                        throw new InvalidArgumentException(addRoleresult.Errors.First().ToString()!);
                    }
                }




                await _repositoryManager.SaveAsync();
                await _repositoryManager.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing moderator status for user with ID {UserId}", userId);
                await _repositoryManager.RollbackTransactionAsync();
                throw;
            }
        }
        public async Task UserBanStatusChange(int userId, Ban ban)
        {
            await _repositoryManager.BeginTransactionAsync();
            try
            {
                var user = await _repositoryManager.UserRepository.Users()
                    .Where(u => u.Id == userId && !u.Roles.Any(r=>r.RoleId == -2))
                    .SingleOrDefaultAsync(); // Use SingleOrDefault for unique lookup
                    
                if (user == null)
                {
                    _logger.LogWarning("User not found with ID {UserId}", userId);
                    throw new NotFoundException("User not found");
                }

                user.Banned = ban;
                user.SecurityStamp = Guid.NewGuid().ToString();

                await _repositoryManager.UserRepository.UpdateUser(user);
                await _repositoryManager.SaveAsync();
                await _repositoryManager.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing ban status for user with ID {UserId}", userId);
                await _repositoryManager.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
