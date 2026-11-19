using System;

namespace api.src.modules.auth.domain.interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateToken(Guid userId, int userType, string expiration);
    }
}
