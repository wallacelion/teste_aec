using api.src.modules.address.infrastructure.external.dtos;

namespace api.src.modules.address.infrastructure.external.interfaces
{
    /// <summary>
    /// Interface para consumo da API ViaCEP
    /// </summary>
    public interface IViaCepService
    {
        /// <summary>
        /// Busca informações de endereço pelo CEP
        /// </summary>
        /// <param name="cep">CEP a ser consultado (com ou sem formatação)</param>
        /// <returns>Dados do endereço ou null se não encontrado</returns>
        Task<ViaCepResponseDto?> GetAddressByCepAsync(string cep);

        /// <summary>
        /// Valida se o CEP está no formato correto
        /// </summary>
        /// <param name="cep">CEP a ser validado</param>
        /// <returns>True se válido, False caso contrário</returns>
        bool IsValidCep(string cep);
    }
}
