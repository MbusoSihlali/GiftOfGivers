namespace GiftOfGivers.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.UtcNow;
            
            _logger.LogInformation("Incoming request: {Method} {Path} from {RemoteIpAddress}",
                context.Request.Method,
                context.Request.Path,
                context.Connection.RemoteIpAddress);

            try
            {
                await _next(context);
            }
            finally
            {
                var elapsed = DateTime.UtcNow - startTime;
                _logger.LogInformation("Outgoing response: {StatusCode} for {Method} {Path} - Elapsed: {ElapsedMilliseconds}ms",
                    context.Response.StatusCode,
                    context.Request.Method,
                    context.Request.Path,
                    elapsed.TotalMilliseconds);
            }
        }
    }
}