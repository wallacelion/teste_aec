using api.src.modules.address.application.dtos;
using api.src.modules.address.domain.entities;
using api.src.modules.user.domain.enums;

namespace api.src.modules.user.application.dtos
{
    public record UserDto
    {
        public int Id { get; set; }
        public Guid Uid { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int? Role { get; set; }
        public Address? Address { get; set; }

        public UserDto() { }

        public UserDto(Guid uid, string? name, string? email, string? password, int role = 1)
        {
            Uid = uid;
            if (!string.IsNullOrWhiteSpace(name)) Name = name;
            if (!string.IsNullOrWhiteSpace(email)) Email = email;
            if (!string.IsNullOrWhiteSpace(password)) Password = password;
            Role = role;
        }

        public UserDto(Guid uid, string name, string email, int role)
        {
            Uid = uid;
            Name = name;
            Email = email;
            Role = role;
        }

        public UserDto(int id, Guid uid, string name, string email, int role)
        {
            Id = id;
            Uid = uid;
            Name = name;
            Email = email;
            Role = role;
        }

        public UserDto(Guid uid, string name, string email, int role, Address address)
        {
            Uid = uid;
            Name = name;
            Email = email;
            Role = role;
            Address = address;
        }

        public UserDto(int id, Guid uid, string name, string email, string password, int role)
        {
            Id = id;
            Uid = uid;
            Name = name;
            Email = email;
            Password = password;
            Role = role;
        }
    }
}
