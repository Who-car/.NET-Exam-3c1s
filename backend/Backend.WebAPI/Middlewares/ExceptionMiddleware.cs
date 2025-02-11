using System.Text.Json;
using Backend.Domain.Exceptions;

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
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            if (context.Response.HasStarted)
                throw;

            var statusCode = e switch
            {
                EntityNotFoundException => 404,
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