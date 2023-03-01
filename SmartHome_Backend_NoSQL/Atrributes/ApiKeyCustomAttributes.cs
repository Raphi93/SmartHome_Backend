using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SmartHome_Backend_NoSQL.Models;
using Microsoft.Extensions.Options;
using SmartHome_Backend_NoSQL.Service;
using Microsoft.AspNetCore.Authorization;

namespace ApiKeyCustomAttributes.Attributes
{
    /// <summary>
    /// Atribut Class für API Key
    /// Hier werden die Targets bestummen.
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Method | AttributeTargets.Class)]

    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string APIKEYNAME = "ApiKey";
        /// <summary>
        /// Methode um die API Key zu überprüfen
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns>Authoriation</returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var endpoint = context.HttpContext.GetEndpoint();
            var isAllowAnonymous = endpoint?.Metadata.Any(x => x.GetType() == typeof(AllowAnonymousAttribute));

            if (isAllowAnonymous == true)
            {
                await next();
                return;
            }

            if (!context.HttpContext.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 404,
                    Content = "Not Found"
                };
                return;
            }

            var appSettings = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = appSettings.GetValue<string>(APIKEYNAME);

            if (!apiKey.Equals(extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 404,
                    Content = "Not Found"
                };
                return;
            }
            await next();
        }
    }
}