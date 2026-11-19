using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.src.modules.auth.application.dtos;
using api.src.modules.auth.application.services;

namespace api.src.modules.auth
{
    /// <summary>
    /// Autenticação e autorização
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Realiza login e retorna tokens de acesso
        /// </summary>
        /// <param name="loginRequest">Credenciais de login (email e senha)</param>
        /// <returns>Tokens de acesso e refresh</returns>
        /// <response code="200">Sucesso - Retorna os tokens JWT</response>
        /// <response code="401">Não autorizado - Credenciais inválidas</response>
        /// <response code="400">Requisição inválida - Dados incorretos</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] AuthLoginDto loginRequest)
        {
            
            AuthDto result = await _authService.LoginAsync(loginRequest);
            
            if (result == null)
            {
                return Unauthorized();
            }
            return Ok(result);
        }
    }
}
