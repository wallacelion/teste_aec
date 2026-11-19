namespace api.src.shared.application.dtos
{
    public record UserFilterRequestDto(
        string? Name = null,
        string? Email = null,
        string? OrderBy = "Name",
        string? OrderDirection = "ASC",
        int PageNumber = 1,
        int PageSize = 10
    );

    public record PageResponseDto<T>(
        System.Collections.Generic.IEnumerable<T> Items,
        int TotalItems,
        int PageNumber,
        int PageSize
    );
}
