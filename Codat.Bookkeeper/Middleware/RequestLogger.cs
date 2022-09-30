using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Codat.Bookkeeper.Middleware
{
    public class RequestLogger : IMiddleware
    {
        private readonly ILogger<RequestLogger> _logger;

        public RequestLogger(ILogger<RequestLogger> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var sw = Stopwatch.StartNew();
            await next(context);
            sw.Stop();

            _logger.LogInformation("{Path} {Headers} {StatusCode} {ElapsedMs}",
                context.Request.Path,
                context.Request.Headers.Select(x => $"{x.Key}:{x.Value}"),
                context.Response.StatusCode,
                sw.ElapsedMilliseconds);
        }
    }
}
