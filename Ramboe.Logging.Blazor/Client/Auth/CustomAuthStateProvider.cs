using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Ramboe.Logging.Blazor.Client.Auth;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    readonly ILocalStorageService _localStorageService;

    public CustomAuthStateProvider(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    AuthenticationState Anonymous => new(new ClaimsPrincipal(new ClaimsIdentity()));

    /// <summary>
    /// will be called whenever an <AuthorizedView> is entered
    /// </summary>
    /// <returns></returns>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        //assumes LoginForm.razor successfully stored a token inside our local storage
        var TokenFromStorage = await _localStorageService.GetItemAsync<string>(Constants.STORAGE_TOKENKEY);

        return string.IsNullOrEmpty(TokenFromStorage)
            ? Anonymous// = not authenticated
            : BuildLoggedInAuthenticationState(TokenFromStorage);
    }

    /// <summary>
    /// Builds the authentication state for the current user, so we can read the token's claims from it (like username, id, firstName, lastName, ...)
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public AuthenticationState BuildLoggedInAuthenticationState(string token)
    {
        var claimList = readClaimsFromToken(token);

        #region CAUTION!
        //during to the current IS4 configuration, the authentication type is not 'jwt', but 'at+jwt'
        var authenticationStaste = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claimList, "at+jwt")));
        #endregion

        return authenticationStaste;
    }

    IEnumerable<Claim> readClaimsFromToken(string jwt)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        //decode the token from SHA256
        var token = tokenHandler.ReadJwtToken(jwt);

        //extract the token's claims
        IEnumerable<Claim> claims = token.Claims;

        return claims;
    }
}
