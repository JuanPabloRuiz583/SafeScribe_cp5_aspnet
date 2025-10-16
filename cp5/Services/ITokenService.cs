using cp5.models;

namespace cp5.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
