using System.Net;
using System.Text.Json;
using Application.Core;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate Next;
        private readonly ILogger<ExceptionMiddleware> Logger ;
        private readonly IHostEnvironment Env ;
        public ExceptionMiddleware(RequestDelegate next,ILogger<ExceptionMiddleware> logger,IHostEnvironment env )
        {
            this.Env = env;
            this.Logger = logger;
            this.Next = next;
            

        }

        public async Task InvokeAsync(HttpContext context){
            try{
                await Next(context);
            }catch(Exception ex){
                
                Logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = Env.IsDevelopment()?
                new AppException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                :new AppException(context.Response.StatusCode,"Internal Server Error");

                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

                var json = JsonSerializer.Serialize(response,options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}