using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using api.src.modules.address.infrastructure.external.dtos;
using api.src.modules.address.infrastructure.external.interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Timeout;

namespace api.src.modules.address.infrastructure.external.services
{
    /// <summary>
    /// Serviço de integração com a API ViaCEP
    /// Implementa timeout, retry e fallback
    /// </summary>
    public class ViaCepService : IViaCepService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ViaCepService> _logger;
        private const string BASE_URL = "https://viacep.com.br/ws";
        private const int TIMEOUT_SECONDS = 5;
        private const int MAX_RETRIES = 3;

        public ViaCepService(HttpClient httpClient, ILogger<ViaCepService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            // Configurar timeout no HttpClient
            _httpClient.Timeout = TimeSpan.FromSeconds(TIMEOUT_SECONDS);
        }

        /// <summary>
        /// Busca endereço pelo CEP com retry e fallback
        /// </summary>
        public async Task<ViaCepResponseDto?> GetAddressByCepAsync(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
            {
                _logger.LogWarning("CEP vazio ou nulo fornecido");
                return null;
            }

            // Remove formatação do CEP
            cep = CleanCep(cep);

            if (!IsValidCep(cep))
            {
                _logger.LogWarning("CEP inválido: {Cep}", cep);
                return null;
            }

            // Política de retry com exponential backoff
            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .Or<TimeoutException>()
                .WaitAndRetryAsync(
                    MAX_RETRIES,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(
                            exception,
                            "Tentativa {RetryCount} de {MaxRetries} falhou ao consultar CEP {Cep}. Aguardando {TimeSpan}s antes de tentar novamente.",
                            retryCount,
                            MAX_RETRIES,
                            cep,
                            timeSpan.TotalSeconds
                        );
                    }
                );

            // Política de timeout
            var timeoutPolicy = Policy.TimeoutAsync(
                TimeSpan.FromSeconds(TIMEOUT_SECONDS),
                TimeoutStrategy.Pessimistic
            );

            // Combinar políticas: timeout + retry
            var combinedPolicy = Policy.WrapAsync(retryPolicy, timeoutPolicy);

            try
            {
                // Executar com as políticas aplicadas
                var result = await combinedPolicy.ExecuteAsync(async () =>
                {
                    _logger.LogInformation("Consultando CEP {Cep} na API ViaCEP", cep);

                    var url = $"{BASE_URL}/{cep}/json/";
                    var response = await _httpClient.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError("Erro HTTP {StatusCode} ao consultar CEP {Cep}", response.StatusCode, cep);
                        throw new HttpRequestException($"Erro HTTP: {response.StatusCode}");
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var viaCepResponse = JsonSerializer.Deserialize<ViaCepResponseDto>(
                        content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    if (viaCepResponse == null)
                    {
                        _logger.LogError("Resposta nula ao deserializar CEP {Cep}", cep);
                        throw new InvalidOperationException("Resposta da API inválida");
                    }

                    if (viaCepResponse.Erro)
                    {
                        _logger.LogWarning("CEP {Cep} não encontrado na base de dados ViaCEP", cep);
                        return null;
                    }

                    _logger.LogInformation("CEP {Cep} consultado com sucesso", cep);
                    return viaCepResponse;
                });

                return result;
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Timeout ao consultar CEP {Cep} após {Timeout}s", cep, TIMEOUT_SECONDS);
                return CreateFallbackResponse(cep);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro de rede ao consultar CEP {Cep}", cep);
                return CreateFallbackResponse(cep);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao consultar CEP {Cep}", cep);
                return CreateFallbackResponse(cep);
            }
        }

        /// <summary>
        /// Valida formato do CEP
        /// </summary>
        public bool IsValidCep(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
                return false;

            var cleanCep = CleanCep(cep);

            // CEP deve ter exatamente 8 dígitos
            return Regex.IsMatch(cleanCep, @"^\d{8}$");
        }

        /// <summary>
        /// Remove formatação do CEP (hífens, pontos, espaços)
        /// </summary>
        private string CleanCep(string cep)
        {
            return Regex.Replace(cep, @"[^\d]", string.Empty);
        }

        /// <summary>
        /// Cria resposta de fallback quando a API ViaCEP não está disponível
        /// Retorna um objeto com apenas o CEP preenchido
        /// </summary>
        private ViaCepResponseDto? CreateFallbackResponse(string cep)
        {
            _logger.LogWarning(
                "Usando fallback para CEP {Cep}. Os dados de endereço deverão ser preenchidos manualmente.",
                cep
            );

            // Retorna null para indicar que o CEP deve ser preenchido manualmente
            // Isso permite que o usuário continue o cadastro mesmo com a API offline
            return null;
        }
    }
}
