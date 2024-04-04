using RSAllies.Api.Extensions;
namespace RSAllies.Api.Authentication
{
    public class ApiKeyEndPointFilter(IConfiguration _configuration) : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(AuthenticationConstants.ApiKeyHeaderName, out var extractedApiKey))
            {
                return new UnAuthorizedHttpObjectResult("Api Key is Missing");
            }

            var apiKey = _configuration.GetValue<string>(AuthenticationConstants.ApiKeySectionName);

            if (apiKey != extractedApiKey)
            {
                return new UnAuthorizedHttpObjectResult("Invalid Api Key");
            }

            return await next(context);

        }
    }
}
