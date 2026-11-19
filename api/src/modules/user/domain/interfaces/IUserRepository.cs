using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using api.src.modules.user.domain.entities;

namespace api.src.modules.user.domain.interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserAsync(Guid uid);

        Task<User> GetUserByEmailAsync(string email);
        // Paginação e filtros para listagem de usuários
        Task<(IEnumerable<User> Items, long Total)> GetAllUsersAsync(api.src.shared.application.dtos.UserFilterRequestDto filter);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(Guid uid);
        Task<bool> ValidatePassword(User user, string password);
    }
}
