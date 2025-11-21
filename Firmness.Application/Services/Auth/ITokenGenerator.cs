using Firmness.Domain.Entities;

namespace Firmness.Application.Services.Auth;

public interface ITokenGenerator
{
    string GenerateToken(Client user, IList<string> roles);
}