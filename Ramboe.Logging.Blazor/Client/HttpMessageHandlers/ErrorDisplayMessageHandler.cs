using System.Net.Http.Json;
using Ramboe.Logging.Blazor.Shared;

namespace Ramboe.Logging.Blazor.Client.HttpMessageHandlers;

/// <summary>
/// necessary to display exceptions from the api properly in the frontend
/// </summary>
public class ErrorDisplayMessageHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return response;
        }

        var problemDetails = await response.Content.ReadFromJsonAsync<ErrorMessageModel>();

        var message = problemDetails.Message;

        #region would work as well
        // var message = await response.Content.ReadAsStringAsync();
        #endregion

        throw new HttpRequestException("HTTP error: " + message);
    }
}
