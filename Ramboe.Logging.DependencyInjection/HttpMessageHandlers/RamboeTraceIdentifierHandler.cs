using Microsoft.AspNetCore.Http;

namespace Ramboe.Logging.DependencyInjection.HttpMessageHandlers;

/// <summary>
/// This handler must be attached to any http client who communicates with other webservices within the own infrastructure. Keep in mind that those webservices also must be setup to use RamboeLogging properly, in order to have consistent logging entries accross services
/// </summary>
public class RamboeTraceIdentifierHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RamboeTraceIdentifierHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Get the TraceIdentifier from the current HttpContext
        var traceIdentifier = _httpContextAccessor.HttpContext?.TraceIdentifier;

        if (!string.IsNullOrEmpty(traceIdentifier))
        {
            // Add the TraceIdentifier as a custom header in the outgoing request
            request.Headers.Add("X-Trace-Id", traceIdentifier);
        }

        // Continue processing the request
        return await base.SendAsync(request, cancellationToken);
    }
}
