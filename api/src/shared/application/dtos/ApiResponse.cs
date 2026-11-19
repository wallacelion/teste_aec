namespace api.src.shared.application.dtos
{
    public class PaginationDto
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public long TotalItems { get; set; }
    }

    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Status { get; set; }
        public object Data { get; set; }
        public PaginationDto Pagination { get; set; }
        public string Message { get; set; }

        public ApiResponse()
        {
            Status = "Success";
        }
    }
}
