using EcommerceSharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace EcommerceSharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            string message = "sorry, internal server error occurred. Kindly try again";
            int statuscode = (int)HttpStatusCode.InternalServerError;
            string title = "Error";

            try
            {
                await next(context);
                if (context.Response.StatusCode == (int)StatusCodes.Status429TooManyRequests)
                {
                    message = "Too many requests made";
                    statuscode = (int)StatusCodes.Status429TooManyRequests;
                    title = "Warning";
                    await ModifyRequest(context, message, statuscode, title);
                }

                if (context.Response.StatusCode == (int)StatusCodes.Status401Unauthorized)
                {
                    message = "You are not authorized to access.";
                    statuscode = (int)StatusCodes.Status401Unauthorized;
                    title = "Alert";
                    await ModifyRequest(context, message, statuscode, title);
                }

                if (context.Response.StatusCode == (int)StatusCodes.Status403Forbidden)
                {
                    message = "You are not allowed/required to access";
                    statuscode = (int)StatusCodes.Status403Forbidden;
                    title = "Out of Access";
                    await ModifyRequest(context, message, statuscode, title);
                }
            }

            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                if(ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Out of time";
                    message = "request timeout... try again";
                    statuscode = (int)StatusCodes.Status408RequestTimeout;
                }

                await ModifyRequest(context, message, statuscode, title);
            }
        }

        private static async Task ModifyRequest(HttpContext context, string message, int statuscode, string title)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                    Title = title,
                    Status = statuscode,
                    Detail = message
            }), CancellationToken.None);
            return;
        }
    }
}
