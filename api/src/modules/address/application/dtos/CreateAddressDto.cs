namespace api.src.modules.address.application.dtos
{
    public record CreateAddressDto
    {
        public Guid UserUId { get; init; }
        public string Cep { get; init; }
        public string Logradouro { get; init; }
        public string Number { get; init; }
        public string Complemento { get; init; }
        public string Bairro { get; init; }
        public string Localidade { get; init; }
        public string Uf { get; init; }

        public CreateAddressDto() { }

        public CreateAddressDto(Guid userUId, string cep, string logradouro, string number, string complemento, string bairro, string localidade, string uf)
        {
            UserUId = userUId;
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
