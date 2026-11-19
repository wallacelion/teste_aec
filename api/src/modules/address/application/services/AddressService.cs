using api.src.modules.address.application.dtos;
using api.src.modules.address.application.interfaces;
using api.src.modules.address.domain.entities;
using api.src.modules.address.domain.interfaces;
using api.src.modules.address.infrastructure.cache.interfaces;
using api.src.modules.address.infrastructure.external.interfaces;
using api.src.modules.user.application.dtos;
using api.src.modules.user.application.interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace api.src.modules.address.application.services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IUserService _userService;
        private readonly IViaCepService _viaCepService;
        private readonly IViaCepCacheService _cacheService;
        private readonly ILogger<AddressService> _logger;

        public AddressService(
            IAddressRepository addressRepository,
            IUserService userService,
            IViaCepService viaCepService,
            IViaCepCacheService cacheService,
            ILogger<AddressService> logger)
        {
            _addressRepository = addressRepository;
            _userService = userService;
            _viaCepService = viaCepService;
            _cacheService = cacheService;
   
            _logger = logger;
        }

        public async Task<AddressDto> GetCepAsync(string cep)
        {
            AddressDto addressData;
            dynamic address = null;

            try
            {
                address = await _cacheService.GetAsync(cep);

                if (address == null && address.Erro)
                    address = await _viaCepService.GetAddressByCepAsync(cep);
            }
            catch (Exception ex)
            {
                address = await _viaCepService.GetAddressByCepAsync(cep);
            }
            finally 
            {

                if (address == null && address.Erro)
                {
                    addressData = new AddressDto
                    {
                        Cep = null,
                        Logradouro = null,
                        Bairro = null,
                        Localidade = null,
                        Uf = null
                    };
                }
                else
                {
                    addressData = new AddressDto
                    {
                        Cep = Regex.Replace(address.Cep, @"\D", ""),
                        Logradouro = address.Logradouro,
                        Bairro = address.Bairro,
                        Localidade = address.Localidade,
                        Uf = address.Uf
                    };
                }
            }

            return addressData;

        }

        public async Task<AddressDto> GetAddressAsync(int id)
        {
            var address = await _addressRepository.GetAddressAsync(id);
            return new AddressDto(address.Id, address.UserId, Regex.Replace(address.Cep, @"\D", ""), address.Logradouro, address.Number, address.Complemento, address.Bairro, address.Localidade, address.Uf);
        }

        public async Task<AddressDto> GetAddressByUserAsync(int id)
        {
            var address = await _addressRepository.GetAddressByUserAsync(id);
            return new AddressDto(address.Id, address.UserId, Regex.Replace(address.Cep, @"\D", ""), address.Logradouro, address.Number, address.Complemento, address.Bairro, address.Localidade, address.Uf);
        }

        public async Task<api.src.shared.application.dtos.PageResponseDto<AddressDto>> GetAllAddressesAsync(api.src.shared.application.dtos.AddressFilterRequestDto filter)
        {
            var (items, total) = await _addressRepository.GetAllAddressesAsync(filter);

            var dtos = items.Select(address => new AddressDto(address.Id, address.UserId, Regex.Replace(address.Cep, @"\D", ""), address.Logradouro, address.Number, address.Complemento, address.Bairro, address.Localidade, address.Uf)).ToList();

            return new api.src.shared.application.dtos.PageResponseDto<AddressDto>(dtos, (int)total, filter.PageNumber, filter.PageSize);
        }

        public async Task<AddressDto> CreateAddressAsync(AddressDto addressDto)
        {
            _logger.LogInformation("Criando endereço para usuário {UserId} com CEP {Cep}", addressDto.UserId, addressDto.Cep);

            // Verificar se o CEP existe no cache
            var cachedData = await _cacheService.GetAsync(addressDto.Cep);
            var viaCepData = cachedData;

            if (cachedData != null)
            {
                _logger.LogInformation("CEP {Cep} encontrado no cache", addressDto.Cep);
            }
            else if (addressDto.Logradouro == null)
            {
                // Se não estiver no cache, consultar ViaCEP
                _logger.LogInformation("CEP {Cep} não encontrado no cache. Consultando ViaCEP", addressDto.Cep);
                viaCepData = await _viaCepService.GetAddressByCepAsync(addressDto.Cep);
            }
            else
            {
                viaCepData = new infrastructure.external.dtos.ViaCepResponseDto{
                    Cep = Regex.Replace(addressDto.Cep, @"\D", "") ?? string.Empty,
                    Logradouro = addressDto.Logradouro ?? string.Empty,
                    Bairro = addressDto.Bairro ?? string.Empty,
                    Localidade = addressDto.Localidade ?? string.Empty,
                    Uf = addressDto.Uf ?? string.Empty
                };
            }

            // Armazenar no cache se a consulta foi bem-sucedida
            if (viaCepData != null && !viaCepData.Erro)
            {
                try
                {
                    await _cacheService.SetAsync(Regex.Replace(addressDto.Cep, @"\D", ""), viaCepData);
                    _logger.LogInformation("CEP {Cep} armazenado no cache", addressDto.Cep);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao armazenar CEP {Cep} no cache", addressDto.Cep);
                }
            }

            string logradouro, bairro, localidade, uf, complemento;

            if (viaCepData != null)
            {
                // Usar dados do ViaCEP quando disponíveis
                _logger.LogInformation("Dados obtidos do ViaCEP para CEP {Cep}", addressDto.Cep);

                logradouro = !string.IsNullOrWhiteSpace(addressDto.Logradouro) 
                    ? addressDto.Logradouro 
                    : viaCepData.Logradouro ?? string.Empty;

                bairro = !string.IsNullOrWhiteSpace(addressDto.Bairro) 
                    ? addressDto.Bairro 
                    : viaCepData.Bairro ?? string.Empty;

                localidade = !string.IsNullOrWhiteSpace(addressDto.Localidade) 
                    ? addressDto.Localidade 
                    : viaCepData.Localidade ?? string.Empty;

                uf = !string.IsNullOrWhiteSpace(addressDto.Uf) 
                    ? addressDto.Uf 
                    : viaCepData.Uf ?? string.Empty;

                complemento = !string.IsNullOrWhiteSpace(addressDto.Complemento) 
                    ? addressDto.Complemento 
                    : viaCepData.Complemento ?? string.Empty;

            }
            else
            {
                // Fallback: usar dados fornecidos pelo usuário
                _logger.LogWarning("ViaCEP indisponível para CEP {Cep}. Usando dados fornecidos pelo usuário.", addressDto.Cep);

                logradouro = addressDto.Logradouro ?? "";
                complemento = addressDto.Complemento ?? "";
                bairro = addressDto.Bairro ?? "";
                localidade = addressDto.Localidade ?? "";
                uf = addressDto.Uf ?? "";
            }

            var address = await _addressRepository.CreateAddressAsync(
                new Address(
                    addressDto.UserId,
                    Regex.Replace(addressDto.Cep, @"\D", "") ?? "", 
                    logradouro, 
                    addressDto.Number, 
                    complemento, 
                    bairro, 
                    localidade, 
                    uf
                )
            );

            _logger.LogInformation("Endereço {AddressId} criado com sucesso para usuário {UserId}", address.Id, address.UserId);

            return new AddressDto(address.Id, address.UserId, Regex.Replace(address.Cep, @"\D", ""), address.Logradouro, address.Number, address.Complemento, address.Bairro, address.Localidade, address.Uf);
        }

        public async Task<AddressDto> UpdateAddressAsync(int id, AddressDto addressDto)
        {
            _logger.LogInformation("Atualizando endereço {AddressId} com CEP {Cep}", id, addressDto.Cep);

            // Buscar endereço existente
            var existingAddress = await _addressRepository.GetAddressAsync(id);
            if (existingAddress == null)
            {
                _logger.LogWarning("Endereço {AddressId} não encontrado", id);
                //return null;
            }

            // Verificar se o CEP existe no cache
            var cachedData = await _cacheService.GetAsync(Regex.Replace(addressDto.Cep, @"\D", ""));
            var viaCepData = cachedData;

            if (cachedData != null)
            {
                _logger.LogInformation("CEP {Cep} encontrado no cache", addressDto.Cep);
            }
            else
            {
                // Se não estiver no cache, consultar ViaCEP
                _logger.LogInformation("CEP {Cep} não encontrado no cache. Consultando ViaCEP", addressDto.Cep);
                viaCepData = await _viaCepService.GetAddressByCepAsync(Regex.Replace(addressDto.Cep, @"\D", ""));

                // Armazenar no cache se a consulta foi bem-sucedida
                if (viaCepData != null && !viaCepData.Erro)
                {
                    await _cacheService.SetAsync(Regex.Replace(addressDto.Cep, @"\D", ""), viaCepData);
                    _logger.LogInformation("CEP {Cep} armazenado no cache", addressDto.Cep);
                }
            }

            string logradouro, bairro, localidade, uf, complemento;

            if (viaCepData != null)
            {
                // Usar dados do ViaCEP quando disponíveis
                _logger.LogInformation("Dados atualizados obtidos do ViaCEP para CEP {Cep}", addressDto.Cep);

                logradouro = !string.IsNullOrWhiteSpace(addressDto.Logradouro) 
                    ? addressDto.Logradouro 
                    : viaCepData.Logradouro ?? string.Empty;

                bairro = !string.IsNullOrWhiteSpace(addressDto.Bairro) 
                    ? addressDto.Bairro 
                    : viaCepData.Bairro ?? string.Empty;

                localidade = !string.IsNullOrWhiteSpace(addressDto.Localidade) 
                    ? addressDto.Localidade 
                    : viaCepData.Localidade ?? string.Empty;

                uf = !string.IsNullOrWhiteSpace(addressDto.Uf) 
                    ? addressDto.Uf 
                    : viaCepData.Uf ?? string.Empty;

                complemento = !string.IsNullOrWhiteSpace(addressDto.Complemento) 
                    ? addressDto.Complemento 
                    : viaCepData.Complemento ?? string.Empty;
            }
            else
            {
                // Fallback: usar dados fornecidos pelo usuário
                _logger.LogWarning("ViaCEP indisponível para CEP {Cep}. Usando dados fornecidos pelo usuário.", addressDto.Cep);

                logradouro = addressDto.Logradouro ?? existingAddress.Logradouro;
                bairro = addressDto.Bairro ?? existingAddress.Bairro;
                localidade = addressDto.Localidade ?? existingAddress.Localidade;
                uf = addressDto.Uf ?? existingAddress.Uf;
                complemento = addressDto.Complemento ?? existingAddress.Complemento;
            }


            var updatedAddress = new Address(
                addressDto.UserId,
                Regex.Replace(addressDto.Cep, @"\D", ""),
                logradouro,
                addressDto.Number,
                complemento,
                bairro,
                localidade,
                uf
            );

            var address = await _addressRepository.UpdateAddressAsync(addressDto.Id, updatedAddress);

            if (address == null)
            {
                _logger.LogError("Falha ao atualizar endereço {AddressId}", addressDto.Id);
                return null;
            }

            _logger.LogInformation("Endereço {AddressId} atualizado com sucesso", address.Id);

            return new AddressDto(address.Id, address.UserId, Regex.Replace(address.Cep, @"\D", ""), address.Logradouro, address.Number, address.Complemento, address.Bairro, address.Localidade, address.Uf);
        }

        public async Task<bool> DeleteAddressAsync(int id)
        {
            _logger.LogInformation("Excluindo endereço {AddressId}", id);
            var result = await _addressRepository.DeleteAddressAsync(id);

            if (result)
            {
                _logger.LogInformation("Endereço {AddressId} excluído com sucesso", id);
            }
            else
            {
                _logger.LogWarning("Falha ao excluir endereço {AddressId}", id);
            }

            return result;
        }
    }
}
