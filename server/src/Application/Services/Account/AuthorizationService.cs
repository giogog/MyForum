using Contracts;
using Domain.Entities;
using Domain.Exception;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Application.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger<AuthorizationService> _logger;

        public AuthorizationService(
            ITokenGenerator tokenGenerator,
            IRepositoryManager repositoryManager,
            ILogger<AuthorizationService> logger)
        {
            _tokenGenerator = tokenGenerator;
            _repositoryManager = repositoryManager;
            _logger = logger;
        }

        public async Task<LoginResponseDto> Authenticate(Expression<Func<User, bool>> expression)
        {
            var user = await _repositoryManager.UserRepository.GetUser(expression);
            if (user == null)
            {
                _logger.LogWarning("User not found for the provided expression.");
                throw new NotFoundException("User not found.");
            }

            var token = await _tokenGenerator.GenerateToken(user);

            return new LoginResponseDto(user.Id, user.UserName, token);
        }

        public async Task<IdentityResult> Register(RegisterDto registerDto)
        {
            await _repositoryManager.BeginTransactionAsync();

            try
            {
                var user = new User
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                    Name = registerDto.Name,
                    Surname = registerDto.Surname
                };

                var result = await _repositoryManager.UserRepository.CreateUser(user, registerDto.Password);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("User registration failed for {Username}: {Errors}", registerDto.Username, result.Errors);
                    await _repositoryManager.RollbackTransactionAsync();
                    return result;
                }

                var roleAssignmentResult = await AssignRoleToUser(user, "User");
                if (!roleAssignmentResult.Succeeded)
                {
                    await _repositoryManager.RollbackTransactionAsync();
                    return roleAssignmentResult;
                }

                await _repositoryManager.CommitTransactionAsync();
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration for {Username}", registerDto.Username);
                await _repositoryManager.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<IdentityResult> Login(LoginDto loginDto)
        {
            var user = await _repositoryManager.UserRepository.GetUser(user => user.UserName == loginDto.Username);

            if (user == null)
            {
                _logger.LogWarning("User {Username} does not exist.", loginDto.Username);
                return IdentityResult.Failed(new IdentityError { Code = "UserDoesNotExist", Description = "The user does not exist." });
            }

            var passwordCheck = await _repositoryManager.UserRepository.CheckPassword(user, loginDto.Password);
            if (!passwordCheck)
            {
                _logger.LogWarning("Incorrect password for user {Username}.", loginDto.Username);
                return IdentityResult.Failed(new IdentityError { Code = "IncorrectPassword", Description = "Incorrect password." });
            }

            if (!user.EmailConfirmed)
            {
                _logger.LogWarning("Email not confirmed for user {Username}.", loginDto.Username);
                return IdentityResult.Failed(new IdentityError { Code = "MailIsNotConfirmed", Description = "Please confirm your email." });
            }
            if (user.Banned == Ban.Banned)
            {
                _logger.LogWarning("{Username} Is banned.", loginDto.Username);
                return IdentityResult.Failed(new IdentityError { Code = "Ban", Description = "User is banned." });
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> AddNewRole(string newRole)
        {
            await _repositoryManager.BeginTransactionAsync();

            try
            {
                if (await _repositoryManager.UserRepository.UserRoleExists(newRole))
                {
                    _logger.LogWarning("Role {Role} already exists.", newRole);
                    await _repositoryManager.RollbackTransactionAsync();
                    return IdentityResult.Failed(new IdentityError { Code = "AlreadyExists", Description = "Role already exists." });
                }

                var role = new Role
                {
                    Name = newRole,
                    NormalizedName = newRole.ToUpper(),
                };

                var result = await _repositoryManager.UserRepository.CreateUserRole(role);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to create role {Role}: {Errors}", newRole, result.Errors);
                    await _repositoryManager.RollbackTransactionAsync();
                    return result;
                }

                await _repositoryManager.CommitTransactionAsync();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating role {Role}", newRole);
                await _repositoryManager.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<IdentityResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            await _repositoryManager.BeginTransactionAsync();

            try
            {
                var user = await _repositoryManager.UserRepository.GetUser(u => u.Email == resetPasswordDto.Email);
                if (user == null)
                {
                    _logger.LogWarning("User with email {Email} not found.", resetPasswordDto.Email);
                    await _repositoryManager.RollbackTransactionAsync();
                    throw new NotFoundException("User not found.");
                }

                var result = await _repositoryManager.UserRepository.ResetPassword(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to reset password for user {Email}: {Errors}", resetPasswordDto.Email, result.Errors);
                    await _repositoryManager.RollbackTransactionAsync();
                    return result;
                }

                await _repositoryManager.CommitTransactionAsync();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while resetting password for user {Email}", resetPasswordDto.Email);
                await _repositoryManager.RollbackTransactionAsync();
                throw;
            }
        }

        private async Task<IdentityResult> AssignRoleToUser(User user, string roleName)
        {

            if (!await _repositoryManager.UserRepository.UserRoleExists(roleName))
            {
                _logger.LogError("Role {Role} does not exist.", roleName);
                return IdentityResult.Failed(new IdentityError { Code = "RoleNotExists", Description = "Role does not exist." });
            }

            var result = await _repositoryManager.UserRepository.AddToRole(user, roleName);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to assign role {Role} to user {Username}: {Errors}", roleName, user.UserName, result.Errors);
            }

            return result;
        }
    }
}
