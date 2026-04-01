using EmployedNow.Domain.Entities;

namespace EmployedNow.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
