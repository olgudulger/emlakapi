using System.Net;
using System.Text.Json;
using Serilog;

namespace WebAPI.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // normal akışa devam
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex); // hata varsa yakala
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            Log.Error(exception, "Unhandled exception on {Path}", context.Request.Path);
            var response = new
            {
                status = "error",
                message = exception.Message,
                path = context.Request.Path
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var json = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(json);
        }
    }
}
