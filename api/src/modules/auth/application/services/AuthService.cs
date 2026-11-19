using System;
using System.Threading.Tasks;

using api.src.modules.auth.application.dtos;
using api.src.modules.auth.domain.interfaces;
using api.src.modules.user.domain.interfaces;

namespace api.src.modules.auth.application.services
{
    public class AuthService
    {

        private readonly IUserRepository _userRepository;
        private readonly ITokenService _authRepository;
        public AuthService(IUserRepository userRepository, ITokenService authRepository)
        {
            _userRepository = userRepository;
            _authRepository = authRepository;
        }
        public async Task<AuthDto?> LoginAsync(AuthLoginDto request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Username);

            if (user == null || !await _userRepository.ValidatePassword(user, request.Password))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
            
            string token =  await _authRepository.GenerateToken(user.Uid, user.Role, "Token:expiration");
            string tokenRefresh = await _authRepository.GenerateToken(user.Uid, user.Role, "Token:refreshExpiration");
            
            AuthDto userToken = new AuthDto { AccessToken = token, RefreshToken = tokenRefresh };
            return userToken;
        }

    }
}
