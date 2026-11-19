using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using api.src.shared.application.dtos;

namespace api.src.shared.infrastructure.filters
{
    public class ApiResponseResultFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var result = context.Result;

            // Let non-object results pass through (e.g., FileResult)
            if (result is FileResult)
            {
                await next();
                return;
            }

            var apiResponse = new ApiResponse();
            int statusCode = 200;
            object data = null;
            string message = null;

            if (result is ObjectResult obj)
            {
                statusCode = obj.StatusCode ?? 200;
                data = obj.Value;

                // If the action already returned an ApiResponse, skip wrapping
                if (data is ApiResponse)
                {
                    await next();
                    return;
                }
            }
            else if (result is NoContentResult || result is EmptyResult)
            {
                statusCode = 204;
                data = null;
            }
            else if (result is StatusCodeResult sc)
            {
                statusCode = sc.StatusCode;
                data = null;
            }
            else
            {
                // For other result types, proceed without wrapping
                await next();
                return;
            }

            apiResponse.StatusCode = statusCode;
            apiResponse.Status = statusCode >= 400 ? "Error" : "Success";
            apiResponse.Data = data;
            apiResponse.Message = message;

            // Tentativa de ler paginação a partir do header X-Pagination (se presente)
            if (context.HttpContext.Response.Headers.TryGetValue("X-Pagination", out var paginationHeader))
            {
                try
                {
                    var pagination = JsonSerializer.Deserialize<PaginationDto>(paginationHeader);
                    apiResponse.Pagination = pagination;
                }
                catch
                {
                    // ignorar se desserialização falhar
                }
            }

            context.Result = new ObjectResult(apiResponse) { StatusCode = statusCode };
            await next();
        }
    }
}
