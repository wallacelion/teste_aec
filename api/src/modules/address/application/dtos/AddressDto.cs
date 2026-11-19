namespace api.src.modules.address.application.dtos
{
    public record AddressDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Cep { get; set; }
        public string? Logradouro { get; set; }
        public string? Number { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Localidade { get; set; }
        public string? Uf { get; set; }

        public AddressDto() { }

        public AddressDto(int id, int userId, string? cep, string? logradouro, string? number, string? complemento, string? bairro, string? localidade, string? uf)
        {
            Id = id;
            UserId = userId;
            if (!string.IsNullOrWhiteSpace(cep)) Cep = cep;
            if (!string.IsNullOrWhiteSpace(logradouro)) Logradouro = logradouro;
            if (!string.IsNullOrWhiteSpace(number)) Number = number;
            if (!string.IsNullOrWhiteSpace(complemento)) Complemento = complemento;
            if (!string.IsNullOrWhiteSpace(bairro)) Bairro = bairro;
            if (!string.IsNullOrWhiteSpace(localidade)) Localidade = localidade;
            if (!string.IsNullOrWhiteSpace(uf)) Uf = uf;
        }

        public AddressDto(int userId, string cep, string logradouro, string number, string complemento, string bairro, string localidade, string uf)
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
