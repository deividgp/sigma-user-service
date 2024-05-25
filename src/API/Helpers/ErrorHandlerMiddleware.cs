namespace API.Helpers;

public class ErrorHandlerMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            response.StatusCode = error switch
            {
                TokenNotFoundException => (int)HttpStatusCode.NotFound,
                TokenInvalidException => TokenInvalidException.REFRESH_TOKEN_CODE,
                _ => (int)HttpStatusCode.InternalServerError,
            };

            var message = error?.Message;

            var result = JsonSerializer.Serialize(new { message });
            await response.WriteAsync(result);
        }
    }
}
