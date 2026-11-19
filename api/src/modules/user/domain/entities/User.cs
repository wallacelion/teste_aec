using api.src.modules.user.domain.enums;
using System;
using System.Runtime.ConstrainedExecution;

namespace api.src.modules.user.domain.entities
{
    public class User
    {
        public int Id { get; private set; }
        public Guid Uid { get; private set; }
        public string? Name { get; private set; }
        public string? Email { get; private set; }
        public string? Password { get; private set; }
        public int Role { get; private set; }

        protected User() { }

        public User(string name, string email, string password, int role = 1)
        {
            Uid = Guid.NewGuid();
            Name = name;
            Email = email;
            Password = password;
            Role = role;
        }

        public User(Guid uid, string name, string email, int role = 1)
        {
            Uid = uid;
            Name = name;
            Email = email;
            Role = role;
        }

        public User(int id, Guid uid, string? name, string? email, string? password, int role = 1)
        {
            Id = id;
            Uid = uid;
            if (!string.IsNullOrWhiteSpace(name)) Name = name;
            if (!string.IsNullOrWhiteSpace(email)) Email = email;
            if (!string.IsNullOrWhiteSpace(password)) Password = password;
            Role = role;
        }
    }
}
