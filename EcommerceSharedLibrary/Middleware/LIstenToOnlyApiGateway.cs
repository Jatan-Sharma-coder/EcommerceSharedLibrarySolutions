using Microsoft.AspNetCore.Http;
namespace EcommerceSharedLibrary.Middleware
{
    public class LIstenToOnlyApiGateway(RequestDelegate next)
    {
        public async Task Invoke(HttpContext context)
        {
            var signedHeader = context.Request.Headers["Api-Gateway"];

            if (signedHeader.FirstOrDefault() is null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Sorry, service is unavailable");
                return;
            }
            else
            {
                await next(context);
            }
        }
    }
}
