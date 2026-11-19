using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using api.src.modules.address.domain.entities;

namespace api.src.modules.address.domain.interfaces
{
    public interface IAddressRepository
    {
        Task<Address> GetAddressAsync(int id);
        Task<Address> GetAddressByUserAsync(int id);
        Task<Address> GetCepAsync(string cep);
        Task<(IEnumerable<Address> Items, long Total)> GetAllAddressesAsync(api.src.shared.application.dtos.AddressFilterRequestDto filter);
        Task<Address> CreateAddressAsync(Address address);
        Task<Address> UpdateAddressAsync(int id, Address address);
        Task<bool> DeleteAddressAsync(int id);
    }
}
