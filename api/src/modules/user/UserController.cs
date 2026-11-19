using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using api.src.modules.user.application.dtos;
using api.src.modules.user.application.interfaces;
using api.src.shared.application.dtos;
using api.src.modules.address.application.interfaces;

namespace api.src.modules.user
{
    /// <summary>
    /// Gerenciamento de usuários
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController: ControllerBase
    {

        public readonly IUserService _userService;
        public readonly IAddressService _addressService;

        public UserController(IUserService userService, IAddressService addressService)
        {
            _userService = userService;
            _addressService = addressService;
        }

        /// <summary>
        /// Obtém todos os usuários
        /// </summary>
        /// <returns>Lista de usuários</returns>
        /// <response code="200">Sucesso - Retorna a lista de usuários</response>
        /// <response code="401">Não autorizado - Token inválido ou ausente</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserFilterRequestDto filter)
        {
            // Garantir valores padrão
            if (filter == null)
            {
                filter = new UserFilterRequestDto();
            }

            // Limites e sanitização
            const int maxPageSize = 100;
            var pageSize = filter.PageSize <= 0 ? 10 : Math.Min(filter.PageSize, maxPageSize);
            var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;

            var orderBy = string.IsNullOrWhiteSpace(filter.OrderBy) ? "Name" : filter.OrderBy;
            orderBy = orderBy.ToLower() switch
            {
                "email" => "Email",
                _ => "Name",
            };

            var orderDir = string.IsNullOrWhiteSpace(filter.OrderDirection) ? "ASC" : filter.OrderDirection.ToUpper();
            orderDir = orderDir == "DESC" ? "DESC" : "ASC";

            filter = filter with { PageNumber = pageNumber, PageSize = pageSize, OrderBy = orderBy, OrderDirection = orderDir };

            var page = await _userService.GetAllUsersAsync(filter);

            // Construir header X-Pagination que será lido pelo filtro global
            var pagination = new PaginationDto
            {
                CurrentPage = page.PageNumber,
                PageSize = page.PageSize,
                TotalItems = page.TotalItems
            };

            var paginationJson = System.Text.Json.JsonSerializer.Serialize(pagination);
            Response.Headers.Remove("X-Pagination");
            Response.Headers.Add("X-Pagination", paginationJson);

            return Ok(page.Items);
        }

        /// <summary>
        /// Obtém um usuário pelo ID
        /// </summary>
        /// <param name="uid">ID único do usuário (GUID)</param>
        /// <returns>Dados do usuário</returns>
        /// <response code="200">Sucesso - Retorna os dados do usuário</response>
        /// <response code="404">Não encontrado - Usuário não existe</response>
        /// <response code="401">Não autorizado - Token inválido ou ausente</response>
        [HttpGet("{uid:guid}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserById(Guid uid)
        {
            var user = await _userService.GetUserAsync(uid);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        /// <param name="createUserDto">Dados do novo usuário</param>
        /// <returns>Usuário criado</returns>
        /// <response code="201">Criado - Retorna o usuário criado</response>
        /// <response code="400">Requisição inválida - Dados incorretos</response>
        /// <response code="401">Não autorizado - Token inválido ou ausente</response>
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateUser([FromBody] UserDto createUserDto)
        {
            var user = await _userService.CreateUserAsync(createUserDto);
            return CreatedAtAction(nameof(GetUserById), new { uid = user.Uid }, user);
        }

        /// <summary>
        /// Atualiza um usuário existente
        /// </summary>
        /// <param name="uid">ID único do usuário (GUID)</param>
        /// <param name="updateUserDto">Dados atualizados do usuário</param>
        /// <returns>Usuário atualizado</returns>
        /// <response code="200">Sucesso - Retorna o usuário atualizado</response>
        /// <response code="404">Não encontrado - Usuário não existe</response>
        /// <response code="400">Requisição inválida - Dados incorretos</response>
        /// <response code="401">Não autorizado - Token inválido ou ausente</response>
        [HttpPatch("{uid:guid}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateUser(Guid uid, [FromBody] UserDto updateUserDto)
        {
            var user = await _userService.UpdateUserAsync(uid, updateUserDto);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        /// <summary>
        /// Exclui um usuário
        /// </summary>
        /// <param name="uid">ID único do usuário (GUID)</param>
        /// <returns>Sem conteúdo</returns>
        /// <response code="204">Sucesso - Usuário excluído</response>
        /// <response code="404">Não encontrado - Usuário não existe</response>
        /// <response code="401">Não autorizado - Token inválido ou ausente</response>
        [HttpDelete("{uid:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteUser(Guid uid)
        {
            var result = await _userService.DeleteUserAsync(uid);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
