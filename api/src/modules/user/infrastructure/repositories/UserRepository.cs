using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using api.src.modules.user.domain.entities;
using api.src.modules.user.infrastructure.context;
using api.src.modules.user.domain.interfaces;

namespace api.src.modules.user.infrastructure.repositories
{
    public class UserRepository: IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserAsync(Guid uid)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Uid == uid && u.Role == 1);
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<(IEnumerable<User> Items, long Total)> GetAllUsersAsync(api.src.shared.application.dtos.UserFilterRequestDto filter)
        {
            var query = _context.Users.AsQueryable();

            // Filtros básicos
            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                var name = filter.Name.ToLower();
                query = query.Where(u => u.Name.ToLower().Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(filter.Email))
            {
                var email = filter.Email.ToLower();
                query = query.Where(u => u.Email.ToLower().Contains(email));
            }

            // Contagem total antes de aplicar Skip/Take
            var total = await query.LongCountAsync();

            // Ordenação
            var orderBy = (filter.OrderBy ?? "Name").ToLower();
            var orderDir = (filter.OrderDirection ?? "ASC").ToUpper();

            // Ordenação simples
            if (orderBy == "email")
            {
                query = orderDir == "DESC" ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email);
            }
            else // default: Name
            {
                query = orderDir == "DESC" ? query.OrderByDescending(u => u.Name) : query.OrderBy(u => u.Name);
            }

            var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            var items = await query.Where(u => u.Role == 1).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, total);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(Guid uid)
        {
            var user = await GetUserAsync(uid);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ValidatePassword(User user, string password)
        {
            var passwordHasher = new PasswordHasher<string>();
            var resultado = passwordHasher.VerifyHashedPassword("usuario_contexto", user.Password, password);
            return resultado == PasswordVerificationResult.Success;
        }
    }
}
