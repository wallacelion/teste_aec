namespace api.src.shared.application.dtos
{
    public record AddressFilterRequestDto(
        string? Cep = null,
        string? Logradouro = null,
        string? Bairro = null,
        string? Localidade = null,
        string? OrderBy = "Id",
        string? OrderDirection = "ASC",
        int PageNumber = 1,
        int PageSize = 10
    );
}
