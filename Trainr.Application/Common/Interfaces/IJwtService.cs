namespace Trainr.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(string userId, string email, string firstName, string lastName, string role);
}
