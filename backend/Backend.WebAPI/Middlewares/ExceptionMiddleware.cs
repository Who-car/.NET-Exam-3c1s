using System.Text.Json;

namespace Backend.WebAPI.Middlewares;

public class ExceptionMiddleware(IWebHostEnvironment hostingEnvironment) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            if (context.Response.HasStarted)
                throw;

            var statusCode = e switch
            {
                _ => 500
            };

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                Message = e.Message,
                Exception = SerializeException(e)
            };

            var body = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(body);
        }
    }
    
    private string? SerializeException(Exception e) => 
        hostingEnvironment.IsProduction() ? null : e.ToString();
}