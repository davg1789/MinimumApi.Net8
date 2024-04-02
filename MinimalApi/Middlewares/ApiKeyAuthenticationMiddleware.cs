using Microsoft.AspNetCore.Components.Forms;
using MinimalApi.Conf;
using System.Net;

namespace MinimalApi.Middlewares
{
    public class ApiKeyAuthenticationMiddleware(RequestDelegate next, IConfiguration config)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            if(!context.Request.Headers.TryGetValue(ApiConstants.ApiHeaderKey, out var actualApiKey))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Api key is missing.");
                return;
            }

            if (!actualApiKey.Equals(config.GetValue<string>(ApiConstants.ApiSecret)))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Api key is invalid.");
                return;
            }

            await next(context);
        }
    }
}
