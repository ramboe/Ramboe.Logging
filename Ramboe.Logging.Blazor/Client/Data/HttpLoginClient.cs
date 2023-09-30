using System.Text;
using System.Text.Json;
using Ramboe.Logging.Blazor.Shared;

namespace Ramboe.Logging.Blazor.Client.Data;

public class HttpLoginClient
{
    readonly HttpClient _client;

    public HttpLoginClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<LoginTokenModel> Login(string username, string password)
    {
        var loginModel = new LoginModel
        {
            Email = username,
            Password = password
        };

        var stringContent = JsonSerializer.Serialize(loginModel);

        var httpRequestMessage = new HttpRequestMessage
        {
            Content = new StringContent(stringContent, Encoding.UTF8, "application/json"),
            RequestUri = new Uri(_client.BaseAddress + "login"),
            Method = HttpMethod.Post
        };

        //debug
        var requestUri = httpRequestMessage.RequestUri.ToString();

        // Send the POST request to the token endpoint
        var response = await _client.SendAsync(httpRequestMessage);

        // Check if the response is successful
        if (response.IsSuccessStatusCode is not true)
        {
            throw new Exception($"{response.StatusCode} - {response.ReasonPhrase}");
        }

        // Read and display the response content (access token)
        var resp = await response.Content.ReadAsStringAsync();

        var opt = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        var accessTokenModel = JsonSerializer.Deserialize<LoginTokenModel>(resp, opt);

        return accessTokenModel;
    }
}
