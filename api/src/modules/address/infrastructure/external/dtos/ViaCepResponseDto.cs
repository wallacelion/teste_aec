namespace api.src.modules.address.infrastructure.external.dtos
{
    /// <summary>
    /// DTO de resposta da API ViaCEP
    /// </summary>
    public class ViaCepResponseDto
    {
        public string? Cep { get; set; }
        public string? Logradouro { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Localidade { get; set; }
        public string? Uf { get; set; }
        public string? Ibge { get; set; }
        public string? Gia { get; set; }
        public string? Ddd { get; set; }
        public string? Siafi { get; set; }

        /// <summary>
        /// Indica se houve erro na consulta
        /// </summary>
        public bool Erro { get; set; }

        public ViaCepResponseDto() { }

        public ViaCepResponseDto(string? cep, string? logradouro, string? complemento, string? bairro, string? localidade, string? uf, string? ibge, string? gia, string? ddd, string? siafi, bool erro)
        {
            Cep = cep;
            Logradouro = logradouro;
            Complemento = complemento;
            Bairro = bairro;
            Localidade = localidade;
            Uf = uf;
            Ibge = ibge;
            Gia = gia;
            Ddd = ddd;
            Siafi = siafi;
            Erro = erro;
        }
    }
}
