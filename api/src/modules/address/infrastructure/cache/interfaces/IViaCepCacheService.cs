using api.src.modules.address.infrastructure.external.dtos;

namespace api.src.modules.address.infrastructure.cache.interfaces
{
    /// <summary>
    /// Interface para cache de consultas ViaCEP
    /// </summary>
    public interface IViaCepCacheService
    {
        /// <summary>
        /// Obtém dados do CEP do cache
        /// </summary>
        /// <param name="cep">CEP a ser consultado</param>
        /// <returns>Dados do cache ou null se não existir</returns>
        Task<ViaCepResponseDto?> GetAsync(string cep);

        /// <summary>
        /// Armazena dados do CEP no cache
        /// </summary>
        /// <param name="cep">CEP a ser armazenado</param>
        /// <param name="data">Dados do endereço</param>
        /// <param name="expirationMinutes">Tempo de expiração em minutos (padrão: 1440 = 24h)</param>
        Task SetAsync(string cep, ViaCepResponseDto data, int expirationMinutes = 1440);

        /// <summary>
        /// Remove dados do CEP do cache
        /// </summary>
        /// <param name="cep">CEP a ser removido</param>
        Task RemoveAsync(string cep);

        /// <summary>
        /// Verifica se o CEP existe no cache
        /// </summary>
        /// <param name="cep">CEP a ser verificado</param>
        /// <returns>True se existir, False caso contrário</returns>
        Task<bool> ExistsAsync(string cep);
    }
}
