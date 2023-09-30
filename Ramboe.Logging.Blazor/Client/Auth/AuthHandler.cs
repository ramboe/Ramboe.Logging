using System.Net;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace Ramboe.Logging.Blazor.Client.Auth;

/// <summary>
///     Not usabe in Blazor Server due to JavaScriptInterop exception. For some reason it is possible to call JavaScript
///     from the HttpClient but not from the delegatingHandler
/// </summary>
public class AuthHandler : DelegatingHandler
{
    readonly ILocalStorageService _localStorageService;
    readonly NavigationManager _navigationManager;

    public AuthHandler(NavigationManager navigationManager, ILocalStorageService localStorageService)
    {
        _navigationManager = navigationManager;
        _localStorageService = localStorageService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _localStorageService.GetItemAsStringAsync(Constants.STORAGE_TOKENKEY);

        request.Headers.Add("Authorization", "Bearer " + token);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return response;
        }

        await Task.Delay(3000);

        _navigationManager.NavigateTo(_navigationManager.BaseUri, true);

        return response;
    }
}
