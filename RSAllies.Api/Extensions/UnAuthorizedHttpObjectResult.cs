namespace RSAllies.Api.Extensions
{
    public sealed class UnAuthorizedHttpObjectResult : IResult, IStatusCodeHttpResult
    {
        private readonly Object _body;

        public UnAuthorizedHttpObjectResult(Object body)
        {
            _body = body;
        }
        public int StatusCode => StatusCodes.Status401Unauthorized;

        int? IStatusCodeHttpResult.StatusCode => StatusCode;

        public async Task ExecuteAsync(HttpContext httpContext)
        {
            ArgumentNullException.ThrowIfNull(httpContext);

            httpContext.Response.StatusCode = StatusCode;
            if (_body is string s)
            {
                await httpContext.Response.WriteAsync(s);
                return;
            }

            await httpContext.Response.WriteAsJsonAsync(_body);

        }
    }
}
