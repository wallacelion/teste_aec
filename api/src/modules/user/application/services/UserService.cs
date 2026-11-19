using api.src.modules.address.domain.interfaces;
using api.src.modules.user.application.dtos;
using api.src.modules.user.application.interfaces;
using api.src.modules.user.domain.entities;
using api.src.modules.user.domain.interfaces;
using api.src.shared.application.dtos;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace api.src.modules.user.application.services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;

        public UserService(IUserRepository userRepository, IAddressRepository addressRepository)
        {
            _userRepository = userRepository;
            _addressRepository = addressRepository;
        }

        public async Task<UserDto> GetUserAsync(Guid uid)
        {
            var user = await _userRepository.GetUserAsync(uid);
            var address = await _addressRepository.GetAddressByUserAsync(user.Id);
            return new UserDto(user.Uid, user.Name, user.Email, user.Role, address);
        }

        public async Task<UserDto> GetUserWithIdAsync(Guid uid, bool showPassword = false)
        {
            var user = await _userRepository.GetUserAsync(uid);
            return showPassword ? new UserDto(user.Id, user.Uid, user.Name, user.Email, user.Password, user.Role) : new UserDto(user.Id, user.Uid, user.Name, user.Email, user.Role);
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            return new UserDto(user.Uid, user.Name, user.Email, user.Role);
        }

        public async Task<api.src.shared.application.dtos.PageResponseDto<UserDto>> GetAllUsersAsync(api.src.shared.application.dtos.UserFilterRequestDto filter)
        {
            var (items, total) = await _userRepository.GetAllUsersAsync(filter);

            var dtos = items.Select(user => new UserDto(user.Uid, user.Name, user.Email, user.Role)).ToList();

            return new api.src.shared.application.dtos.PageResponseDto<UserDto>(dtos, (int)total, filter.PageNumber, filter.PageSize);
        }
        public async Task<UserDto> CreateUserAsync(UserDto userDto)
        {
            var passwordHasher = new PasswordHasher<string>();
            var hashedPassword = passwordHasher.HashPassword("user_context", userDto.Password);

            var user = await _userRepository.CreateUserAsync(new User(userDto.Name, userDto.Email, hashedPassword, userDto?.Role ?? 1));
            return new UserDto(user.Uid, user.Name, user.Email, user.Role);
        }
        public async Task<UserDto> UpdateUserAsync(Guid uid, UserDto userDto)
        {

            var userData = await GetUserWithIdAsync(uid, true);

            if (userData is null)
                return null;

            if (!string.IsNullOrWhiteSpace(userDto.Name)) userData.Name = userDto.Name;
            if (!string.IsNullOrWhiteSpace(userDto.Email)) userData.Email = userDto.Email;

            string hashedPassword = userData.Password;

            if (userDto?.Password != null)
            {
                var passwordHasher = new PasswordHasher<string>();
                hashedPassword = passwordHasher.HashPassword("user_context", userDto.Password);
            }

            var user = await _userRepository.UpdateUserAsync(new User(userData.Id, uid, userData.Name, userData.Email, hashedPassword, userDto?.Role ?? 1));
            if (user == null)
            {
                return null;
            }
            return new UserDto(user.Uid, user.Name, user.Email, user.Role);
        }
        public async Task<bool> DeleteUserAsync(Guid uid)
        {

            var user = await _userRepository.GetUserAsync(uid);
            var address = await _addressRepository.GetAddressByUserAsync(user.Id);
            await _addressRepository.DeleteAddressAsync(address.Id);

            return await _userRepository.DeleteUserAsync(uid);
        }

        public async Task<bool> ValidatePassword(User user, string password)
        {
            var passwordHasher = new PasswordHasher<string>();

            var resultado = passwordHasher.VerifyHashedPassword("usuario_contexto", user.Password, password);

            return resultado == PasswordVerificationResult.Success;
        }
    }
}
