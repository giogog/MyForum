using Domain.Entities;

namespace Contracts;

public interface ITokenGenerator
{
    Task<string> GenerateToken(User user);
}
