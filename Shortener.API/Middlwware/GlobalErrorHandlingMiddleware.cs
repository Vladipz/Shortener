using System.Net;
using System.Text.Json;

using Microsoft.IdentityModel.Tokens;

namespace Shortener.API.Middlwware
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Встановлюємо статус код відповідно до типу винятку
            context.Response.StatusCode = exception switch
            {
                SecurityTokenException => (int)HttpStatusCode.Unauthorized, // 401 для помилок токену
                InvalidOperationException => (int)HttpStatusCode.BadRequest,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                ArgumentNullException => (int)HttpStatusCode.BadRequest,
                FormatException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            // Формуємо відповідь
            var response = new
            {
                statusCode = context.Response.StatusCode,
                message = exception.Message,
            };

            var responseJson = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(responseJson);
        }
    }
}