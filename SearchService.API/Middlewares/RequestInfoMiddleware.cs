using SearchService.Application.Dto;

namespace SearchService.API.Middlewares;

public class RequestInfoMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString();
        var userAgent = context.Request.Headers.UserAgent.ToString();
        var referer = context.Request.Headers.Referer.ToString();

        context.Items["RequestInfo"] = new RequestInfoDto(ip, userAgent, referer);

        await next(context);
    }
}