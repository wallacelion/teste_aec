using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using api.src.modules.address.application.dtos;

namespace api.src.modules.address.application.interfaces
{
    public interface IAddressService
    {
        Task<AddressDto> GetAddressAsync(int id);
        Task<AddressDto> GetCepAsync(string cep);
        Task<api.src.shared.application.dtos.PageResponseDto<AddressDto>> GetAllAddressesAsync(api.src.shared.application.dtos.AddressFilterRequestDto filter);
        Task<AddressDto> CreateAddressAsync(AddressDto addressDto);
        Task<AddressDto> UpdateAddressAsync(int id, AddressDto addressDto);
        Task<bool> DeleteAddressAsync(int id);
    }
}
