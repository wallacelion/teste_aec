using api.src.modules.address.application.dtos;
using api.src.modules.address.application.interfaces;
using api.src.modules.address.infrastructure.external.interfaces;
using api.src.modules.user.application.interfaces;
using api.src.shared.application.dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace api.src.modules.address
{
    /// <summary>
    /// Gerenciamento de endereços
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AddressController: ControllerBase
    {

        public readonly IAddressService _addressService;
        public readonly IUserService _userService;
        private readonly IViaCepService _viaCepService;

        public AddressController(IAddressService addressService, IUserService userService, IViaCepService viaCepService)
        {
            _addressService = addressService;
            _userService = userService;
            _viaCepService = viaCepService;
        }

        /// <summary>
        /// Obtém todos os endereços
        /// </summary>
        /// <returns>Lista de endereços</returns>
        /// <response code="200">Sucesso - Retorna a lista de endereços</response>
        /// <response code="401">Não autorizado - Token inválido ou ausente</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AddressDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllAddresses([FromQuery] AddressFilterRequestDto filter)
        {
            if (filter == null) filter = new AddressFilterRequestDto();

            const int maxPageSize = 100;
            var pageSize = filter.PageSize <= 0 ? 10 : Math.Min(filter.PageSize, maxPageSize);
            var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;

            var orderBy = string.IsNullOrWhiteSpace(filter.OrderBy) ? "Id" : filter.OrderBy;
            orderBy = orderBy.ToLower() switch
            {
                "cep" => "Cep",
                "localidade" => "Localidade",
                _ => "Id",
            };

            var orderDir = string.IsNullOrWhiteSpace(filter.OrderDirection) ? "ASC" : filter.OrderDirection.ToUpper();
            orderDir = orderDir == "DESC" ? "DESC" : "ASC";

            filter = filter with { PageNumber = pageNumber, PageSize = pageSize, OrderBy = orderBy, OrderDirection = orderDir };

            var page = await _addressService.GetAllAddressesAsync(filter);

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
        /// Obtém um endereço pelo ID
        /// </summary>
        /// <param name="id">ID do endereço</param>
        /// <returns>Dados do endereço</returns>
        /// <response code="200">Sucesso - Retorna os dados do endereço</response>
        /// <response code="404">Não encontrado - Endereço não existe</response>
        /// <response code="401">Não autorizado - Token inválido ou ausente</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAddressById(int id)
        {
            var address = await _addressService.GetAddressAsync(id);
            if (address == null)
            {
                return NotFound();
            }
            return Ok(address);
        }

        /// <summary>
        /// Obtém um endereço pelo CEP
        /// </summary>
        /// <param name="cep">CEP do endereço</param>
        /// <returns>Dados do endereço</returns>
        /// <response code="200">Sucesso - Retorna os dados do endereço</response>
        /// <response code="404">Não encontrado - Endereço não existe</response>
        /// <response code="401">Não autorizado - Token inválido ou ausente</response>
        [HttpGet("search/{cep}")]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAddressByCep(string cep)
        {
            var address = await _addressService.GetCepAsync(cep);

            if (address == null)
            {
                return NotFound();
            }
            return Ok(address);
        }

        /// <summary>
        /// Cria um novo endereço e salva o cache
        /// </summary>
        /// <param name="createAddressDto">Dados do novo endereço</param>
        /// <returns>Endereço criado</returns>
        /// <response code="201">Criado - Retorna o endereço criado</response>
        /// <response code="400">Requisição inválida - Dados incorretos</response>
        /// <response code="401">Não autorizado - Token inválido ou ausente</response>
        [HttpPost]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateAddress([FromBody] CreateAddressDto createAddressDto)
        {

            var uid = createAddressDto.UserUId;
            var user = await _userService.GetUserWithIdAsync(uid, false);
            AddressDto newAddress = new AddressDto
            {
                UserId = user.Id,
                Cep = createAddressDto.Cep,
                Number = createAddressDto.Number,
                Logradouro = createAddressDto.Logradouro,
                Complemento = createAddressDto.Complemento,
                Bairro = createAddressDto.Bairro,
                Localidade = createAddressDto.Localidade,
                Uf = createAddressDto.Uf
            };
            
            var address = await _addressService.CreateAddressAsync(newAddress);
            return CreatedAtAction(nameof(GetAddressById), new { id = address.Id }, address);
        }

        /// <summary>
        /// Atualiza um endereço existente
        /// </summary>
        /// <param name="id">ID do endereço</param>
        /// <param name="updateAddressDto">Dados atualizados do endereço</param>
        /// <returns>Endereço atualizado</returns>
        /// <response code="200">Sucesso - Retorna o endereço atualizado</response>
        /// <response code="404">Não encontrado - Endereço não existe</response>
        /// <response code="400">Requisição inválida - Dados incorretos</response>
        /// <response code="401">Não autorizado - Token inválido ou ausente</response>
        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddressDto updateAddressDto)
        {
            
            var address = await _addressService.UpdateAddressAsync(id, updateAddressDto);
            if (address == null)
            {
                return NotFound();
            }
            return Ok(address);
        }

        /// <summary>
        /// Exclui um endereço
        /// </summary>
        /// <param name="id">ID do endereço</param>
        /// <returns>Sem conteúdo</returns>
        /// <response code="204">Sucesso - Endereço excluído</response>
        /// <response code="404">Não encontrado - Endereço não existe</response>
        /// <response code="401">Não autorizado - Token inválido ou ausente</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var result = await _addressService.DeleteAddressAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        /// <summary>
        /// Consulta CEP na API ViaCEP
        /// </summary>
        /// <param name="cep">CEP a ser consultado (com ou sem formatação)</param>
        /// <returns>Dados do endereço do ViaCEP</returns>
        /// <response code="200">Sucesso - Retorna os dados do CEP consultado</response>
        /// <response code="404">Não encontrado - CEP inválido ou não encontrado</response>
        /// <response code="401">Não autorizado - Token inválido ou ausente</response>
        /// <response code="408">Timeout - API ViaCEP não respondeu a tempo</response>
        [HttpGet("viacep/{cep}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
        public async Task<IActionResult> ConsultCep(string cep)
        {
            if (!_viaCepService.IsValidCep(cep))
            {
                return BadRequest(new { error = "CEP inválido. Use o formato: 12345678 ou 12345-678" });
            }

            var viaCepData = await _viaCepService.GetAddressByCepAsync(cep);

            if (viaCepData == null)
            {
                return NotFound(new { error = "CEP não encontrado ou serviço ViaCEP temporariamente indisponível" });
            }

            return Ok(viaCepData);
        }
    }
}
