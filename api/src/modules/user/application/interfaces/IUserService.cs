using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using api.src.modules.user.domain.entities;
using api.src.modules.user.application.dtos;
using api.src.shared.application.dtos;

namespace api.src.modules.user.application.interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetUserAsync(Guid uid);
        Task<UserDto> GetUserWithIdAsync(Guid uid, bool showPassword);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<api.src.shared.application.dtos.PageResponseDto<UserDto>> GetAllUsersAsync(api.src.shared.application.dtos.UserFilterRequestDto filter);
        Task<UserDto> CreateUserAsync(UserDto userDto);
        Task<UserDto> UpdateUserAsync(Guid uid, UserDto userDto);
        Task<bool> DeleteUserAsync(Guid uid);
        Task<bool> ValidatePassword(User user, string password);
    }
}
