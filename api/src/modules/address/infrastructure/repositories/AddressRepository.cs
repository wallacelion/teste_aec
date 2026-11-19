using api.src.modules.address.application.dtos;
using api.src.modules.address.domain.entities;
using api.src.modules.address.domain.interfaces;
using api.src.modules.address.infrastructure.context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace api.src.modules.address.infrastructure.repositories
{
    public class AddressRepository: IAddressRepository
    {
        private readonly AddressDbContext _context;

        public AddressRepository(AddressDbContext context)
        {
            _context = context;
        }

        public async Task<Address> GetAddressAsync(int id)
        {
            return await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Address> GetAddressByUserAsync(int id)
        {
            return await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == id);
        }

        public async Task<Address> GetCepAsync(string cep)
        {
            return await _context.Addresses.FirstOrDefaultAsync(a => a.Cep == cep);
        }

        public async Task<(IEnumerable<Address> Items, long Total)> GetAllAddressesAsync(api.src.shared.application.dtos.AddressFilterRequestDto filter)
        {
            var query = _context.Addresses.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Cep))
            {
                var cep = filter.Cep.Replace("-", "").Trim();
                query = query.Where(a => a.Cep.Replace("-", "").Contains(cep));
            }

            if (!string.IsNullOrWhiteSpace(filter.Logradouro))
            {
                var log = filter.Logradouro.ToLower();
                query = query.Where(a => a.Logradouro.ToLower().Contains(log));
            }

            if (!string.IsNullOrWhiteSpace(filter.Bairro))
            {
                var bairro = filter.Bairro.ToLower();
                query = query.Where(a => a.Bairro.ToLower().Contains(bairro));
            }

            if (!string.IsNullOrWhiteSpace(filter.Localidade))
            {
                var local = filter.Localidade.ToLower();
                query = query.Where(a => a.Localidade.ToLower().Contains(local));
            }

            var total = await query.LongCountAsync();

            var orderBy = (filter.OrderBy ?? "Id").ToLower();
            var orderDir = (filter.OrderDirection ?? "ASC").ToUpper();

            if (orderBy == "cep")
            {
                query = orderDir == "DESC" ? query.OrderByDescending(a => a.Cep) : query.OrderBy(a => a.Cep);
            }
            else if (orderBy == "localidade")
            {
                query = orderDir == "DESC" ? query.OrderByDescending(a => a.Localidade) : query.OrderBy(a => a.Localidade);
            }
            else
            {
                query = orderDir == "DESC" ? query.OrderByDescending(a => a.Id) : query.OrderBy(a => a.Id);
            }

            var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, total);
        }

        public async Task<Address> CreateAddressAsync(Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<Address> UpdateAddressAsync(int id, Address address)
        {
            var existingAddress = await GetAddressAsync(id);
            if (existingAddress == null)
            {
                return null;
            }

            // Atualizar propriedades manualmente para evitar problemas com tracking
            _context.Entry(existingAddress).CurrentValues.SetValues(address);

            await _context.SaveChangesAsync();
            return existingAddress;
        }

        public async Task<bool> DeleteAddressAsync(int id)
        {
            var address = await GetAddressAsync(id);
            if (address == null)
            {
                return false;
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
