using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using api.src.modules.address.infrastructure.cache.interfaces;
using api.src.modules.address.infrastructure.external.dtos;

namespace api.src.modules.address.infrastructure.cache.services
{
    /// <summary>
    /// Implementação de cache Redis para consultas ViaCEP
    /// </summary>
    public class ViaCepCacheService : IViaCepCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<ViaCepCacheService> _logger;
        private const string CACHE_KEY_PREFIX = "viacep:";

        public ViaCepCacheService(
            IDistributedCache cache,
            ILogger<ViaCepCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Gera chave do cache para o CEP
        /// </summary>
        private string GetCacheKey(string cep)
        {
            // Remove formatação do CEP para garantir consistência
            var cleanCep = cep.Replace("-", "").Replace(".", "").Trim();
            return $"{CACHE_KEY_PREFIX}{cleanCep}";
        }

        /// <summary>
        /// Obtém dados do CEP do cache Redis
        /// </summary>
        public async Task<ViaCepResponseDto?> GetAsync(string cep)
        {
            try
            {
                var cacheKey = GetCacheKey(cep);
                var cachedData = await _cache.GetStringAsync(cacheKey);

                if (string.IsNullOrEmpty(cachedData))
                {
                    _logger.LogInformation("Cache MISS para CEP {Cep}", cep);
                    return null;
                }

                _logger.LogInformation("Cache HIT para CEP {Cep}", cep);

                var result = JsonSerializer.Deserialize<ViaCepResponseDto>(
                    cachedData,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar CEP {Cep} do cache", cep);
                // Em caso de erro no cache, retorna null para buscar da API
                return null;
            }
        }

        /// <summary>
        /// Armazena dados do CEP no cache Redis
        /// </summary>
        public async Task SetAsync(string cep, ViaCepResponseDto data, int expirationMinutes = 1440)
        {
            try
            {
                if (data == null || data.Erro)
                {
                    _logger.LogWarning("Tentativa de cachear CEP {Cep} inválido ou com erro", cep);
                    return;
                }

                var cacheKey = GetCacheKey(cep);
                var serializedData = JsonSerializer.Serialize(data, new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                });

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expirationMinutes)
                };

                await _cache.SetStringAsync(cacheKey, serializedData, options);

                _logger.LogInformation(
                    "CEP {Cep} armazenado no cache com expiração de {Expiration} minutos",
                    cep,
                    expirationMinutes
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao armazenar CEP {Cep} no cache", cep);
                // Não propaga erro de cache para não quebrar o fluxo principal
            }
        }

        /// <summary>
        /// Remove dados do CEP do cache Redis
        /// </summary>
        public async Task RemoveAsync(string cep)
        {
            try
            {
                var cacheKey = GetCacheKey(cep);
                await _cache.RemoveAsync(cacheKey);

                _logger.LogInformation("CEP {Cep} removido do cache", cep);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover CEP {Cep} do cache", cep);
            }
        }

        /// <summary>
        /// Verifica se o CEP existe no cache Redis
        /// </summary>
        public async Task<bool> ExistsAsync(string cep)
        {
            try
            {
                var cacheKey = GetCacheKey(cep);
                var cachedData = await _cache.GetStringAsync(cacheKey);

                var exists = !string.IsNullOrEmpty(cachedData);

                _logger.LogDebug("Verificação de existência no cache para CEP {Cep}: {Exists}", cep, exists);

                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência de CEP {Cep} no cache", cep);
                return false;
            }
        }
    }
}
