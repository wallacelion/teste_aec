using System;

namespace api.src.modules.address.domain.entities
{
    public class Address
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public string Cep { get; private set; }
        public string Logradouro { get; private set; }
        public string Number { get; private set; }
        public string Complemento { get; private set; }
        public string Bairro { get; private set; }
        public string Localidade { get; private set; }
        public string Uf { get; private set; }

        protected Address() { }

        public Address(int userId, string cep, string logradouro, string number, string complemento, string bairro, string localidade, string uf)
        {
            UserId = userId;
            Cep = cep;
            Logradouro = logradouro;
            Number = number;
            Complemento = complemento;
            Bairro = bairro;
            Localidade = localidade;
            Uf = uf;
        }

        public void Update(int userId, string cep, string logradouro, string number, string complemento, string bairro, string localidade, string uf)
        {
            UserId = userId;
            Cep = cep;
            Logradouro = logradouro;
            Number = number;
            Complemento = complemento;
            Bairro = bairro;
            Localidade = localidade;
            Uf = uf;
        }
    }
}
